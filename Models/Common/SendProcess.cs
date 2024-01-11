using System.Net.Mail;
using System.Net;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Amazon.SimpleNotificationService;
using Amazon;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class SendProcess
	{
        static readonly IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		///	메일 전송
		/// </summary>
		public bool SendEmail(AuthLog_Dto request)
		{
			bool status = true;
			string to = request.EMAIL_ADDRESS!;
			string from = _config.GetValue<string>("Aws:Email:Sender")!;
			string name = _config.GetValue<string>("Aws:Email:Name")!;
			string password = _config.GetValue<string>("Aws:Email:Password")!;
			string host = _config.GetValue<string>("Aws:Email:Host")!;
			int port = _config.GetValue<int>("Aws:Email:Port")!;
			string fromName = _config.GetValue<string>("ServiceName")!;
			string subject = request.TITLE!;
			string body = request.CONTENTS!;

			try
			{
				MailMessage message = new()
				{
					IsBodyHtml = false,
					From = new MailAddress(from, fromName),
					Subject = subject,
					Body = body
				};
				message.To.Add(new MailAddress(to));

				using var client = new SmtpClient(host, port);
				client.Credentials = new NetworkCredential(name, password);
				client.EnableSsl = true;

				try
				{
					client.Send(message);
				}
				catch (Exception ex)
				{
					_exceptionLogDac.Insert_ExceptionLog(ex);
					status = false;
				}
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				status = false;
			}

			return status;
		}

		/// <summary>
		/// SMS 전송
		/// </summary>
		/// <param name="hpNumber"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public async Task<bool> SendHp(string hpNumber, string message)
		{
			bool status = true;

			try
			{
				hpNumber = "+82 " + hpNumber;
				string accessKey = _config.GetValue<string>("Aws:Sns:AccessKey")!;
				string secretKey = _config.GetValue<string>("Aws:Sns:SecretKey")!;
				string topicArn = _config.GetValue<string>("Aws:Sns:TopicArn")!;

				AmazonSimpleNotificationServiceClient snsClient = new(accessKey, secretKey, RegionEndpoint.USWest2);
				SubscribeRequest subscribeRequest = new(topicArn, "sms", hpNumber);
				await snsClient.SubscribeAsync(subscribeRequest, CancellationToken.None);

				PublishRequest pubRequest = new()
				{
					Message = message,
					PhoneNumber = hpNumber
				};

				await snsClient.PublishAsync(pubRequest);
			}
			catch (Exception ex)
			{
				status = false;
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return status;
		}

		/// <summary>
		/// NFT 민팅
		/// </summary>
		/// <param name="settingInfo"></param>
		/// <param name="contractAddress"></param>
		/// <param name="address"></param>
		/// <param name="tokenId"></param>
		/// <returns></returns>
		async public Task<string[]> Minting(Setting_Dto settingInfo, string contractAddress, string address, int tokenId)
		{
            bool status;
            string transactionHash = string.Empty;
            long transactionNonce = 0;

            try
            {
                string motherAddress = settingInfo!.MOTHER_PUBLIC_KEY!;
                string abi = @"[{'inputs':[{'internalType':'address','name':'to','type':'address'},{'internalType':'uint256','name':'tokenId','type':'uint256'}],'name':'externalMint','outputs':[],'stateMutability':'nonpayable','type':'function'}]";
                Account account = new(new EthECKey(settingInfo!.MOTHER_PRIVATE_KEY).GetPrivateKeyAsBytes().ToHex());
                // web3
                Web3 web3 = new(account, settingInfo!.INFURA_URL);
                var contract = web3.Eth.GetContract(abi, contractAddress);

                var function = contract.GetFunction("externalMint");

                // 데이터 암호화
                string requestParams = function.GetData(address, tokenId);

                // Gas 추정을 위한 CallInput 객체 생성
                CallInput callInput = new()
                {
                    From = motherAddress,
                    To = contractAddress,
                    Data = requestParams
                };

                // Gas 추정
                HexBigInteger gasLimit = await web3.Eth.TransactionManager.EstimateGasAsync(callInput);

                // Gas Price 추정
                HexBigInteger gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                gasPrice = new HexBigInteger(new BigInteger((double)gasPrice.Value * 1.2));

                // Call 함수를 사용하여 트랜잭션 실행
                string callResult = await web3.Eth.Transactions.Call.SendRequestAsync(callInput);
                status = !string.IsNullOrEmpty(callResult);

                if (status)
                {
                    // DB 논스
                    long dbNonce = (long)settingInfo.NONCE!;
                    // 네트워크 논스 조회
                    BigInteger networkNonce = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(motherAddress);
                    // 현재 논스(네트워크 논스가 높으면 네트워크 논스를 기준으로 한다. 출금등 다른 트랜잭션이 있을 수 있기때문)
                    long nonce = (long)networkNonce > dbNonce ? (long)networkNonce : dbNonce;

                    // 파라미터 셋팅
                    TransactionInput transactionInput = new()
                    {
                        From = motherAddress,
                        To = contractAddress,
                        Data = requestParams,
                        Value = new HexBigInteger(new BigInteger(0)),
                        Gas = new HexBigInteger(gasLimit),
                        GasPrice = new HexBigInteger(gasPrice),
                        Nonce = new HexBigInteger(nonce)
                    };

                    // 트랜잭션 사인
                    string signHash = await web3.Eth.TransactionManager.SignTransactionAsync(transactionInput);

                    // 트랜잭션 전송
                    transactionHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(signHash);

                    // 논스는 현재 네트워크에 기록된 값 그대로 던져야하며, db에 저장할떄 +1을 한다.
                    transactionNonce = nonce + 1;

                    // 민팅이 완료되었으면 NONCE값을 최신으로 변경 시킨다.
                    Setting_Dto settingDto = new()
                    {
                        NONCE = transactionNonce
					};

                    new Setting_Dac().Update_MintingNonceInfo(settingDto);
                }
            }
            catch (Exception ex)
            {
                _exceptionLogDac.Insert_ExceptionLog(ex);
                status = false;
            }

            string[] result = new string[3];
            result[0] = status.ToString();
			result[1] = transactionHash;
			result[2] = transactionNonce.ToString();

			return result;
		}

		/// <summary>
		/// NFT 민팅 가능 여부 체크
		/// </summary>
		/// <param name="settingInfo"></param>
		/// <param name="contractAddress"></param>
		/// <param name="nftCount"></param>
		/// <returns></returns>
		async public Task<string> CheckMinting(Setting_Dto settingInfo, string contractAddress, int nftCount)
        {
            bool status = true;
            string errorCode = string.Empty;

            if (status)
            {
                try
                {
                    string motherAddress = settingInfo!.MOTHER_PUBLIC_KEY!;
                    string abi = @"[{'inputs': [{'internalType': 'address','name': 'owner','type': 'address'}],'name': 'balanceOf','outputs': [{'internalType': 'uint256','name': '','type': 'uint256'}],'stateMutability': 'view','type': 'function'},{'inputs': [],'name': 'totalSupply','outputs': [{'internalType': 'uint256','name': '','type': 'uint256'}],'stateMutability': 'view','type': 'function'}]";
                    Account account = new(new EthECKey(settingInfo!.MOTHER_PRIVATE_KEY).GetPrivateKeyAsBytes().ToHex());
                    // web3
                    Web3 web3 = new(account, settingInfo.INFURA_URL);

                    // 보유 수량 조회
                    decimal balance = Web3.Convert.FromWei((await web3.Eth.GetBalance.SendRequestAsync(motherAddress)).Value);
                    status = balance > 0;

                    if (status)
                    {
                        var contract = web3.Eth.GetContract(abi, contractAddress);

                        // 총 발행량 조회
                        var function = contract.GetFunction("totalSupply");
                        long totalSupply = (long)await function.CallAsync<BigInteger>();
                         
                        status = totalSupply < nftCount;
                        if (!status)
                        {
							errorCode = "1003";
                        }
                    }
                    else
                    {
						errorCode = "1002";
					}
                }
                catch (Exception ex)
                {
                    _exceptionLogDac.Insert_ExceptionLog(ex);
					errorCode = "8888";
				}
            }

            return errorCode;
        }

		/// <summary>
		///		API호출
		/// </summary>
		/// <param name="method"></param>
		/// <param name="path"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public JObject CallBerrypieApi(string method, string path, string body = "")
		{
			string uri = _config.GetValue<string>("BerrypieApiUrl")! + path;
			string result = string.Empty;

			ApiLog_Dto apiLogDto = new()
			{
				TYPE_CODE = "0001",
				API_URL = uri,
				REQUEST_DATA = body,
				REQUEST_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
			};

			try
			{
	#pragma warning disable SYSLIB0014 // 형식 또는 멤버는 사용되지 않습니다.
				HttpWebRequest? httpRequest = (HttpWebRequest)WebRequest.Create(new Uri(uri));
	#pragma warning restore SYSLIB0014 // 형식 또는 멤버는 사용되지 않습니다.
				httpRequest.Method = method.ToUpper();
				httpRequest.ContentType = "application/json; charset=utf-8";
				httpRequest.Accept = "application/json; charset=utf-8";

				HttpWebResponse? httpResponse;
				Stream? respPostStream;
				StreamReader? readerPost;

				if (method.Equals("GET"))
				{
					try
					{
						httpResponse = (HttpWebResponse)httpRequest.GetResponse();
						respPostStream = httpResponse.GetResponseStream();
						readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("UTF-8"), true);
						result = readerPost.ReadToEnd();
					}
					catch (WebException ex)
					{
						_exceptionLogDac.Insert_ExceptionLog(ex);
						if (ex.Status == WebExceptionStatus.ProtocolError)
						{
							if (ex.Response is HttpWebResponse response)
							{
								using StreamReader streamReader = new(response.GetResponseStream());
								result = streamReader.ReadToEnd();
							}
						}
					}
				}
				else
				{
					using StreamWriter streamWriter = new(httpRequest.GetRequestStream());
					streamWriter.Write(body);
					streamWriter.Flush();
					streamWriter.Close();

					try
					{
						httpResponse = (HttpWebResponse)httpRequest.GetResponse();
						respPostStream = httpResponse.GetResponseStream();
						readerPost = new StreamReader(respPostStream, Encoding.GetEncoding("UTF-8"), true);
						result = readerPost.ReadToEnd();
					}
					catch (WebException ex)
					{
						_exceptionLogDac.Insert_ExceptionLog(ex);
						if (ex.Status == WebExceptionStatus.ProtocolError)
						{
							if (ex.Response is HttpWebResponse response)
							{
								using StreamReader streamReader = new(response.GetResponseStream());
								result = streamReader.ReadToEnd();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			apiLogDto.RESPONSE_DATA = result;
			apiLogDto.RESPONSE_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			new ApiLog_Dac().Insert_ApiLog(apiLogDto);

			return JObject.Parse(result);
		}
	}
}