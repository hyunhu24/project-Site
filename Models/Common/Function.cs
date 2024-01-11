using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public static partial class Function
    {
		static readonly IConfiguration _config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
		static readonly string _fileBaseUrl = _config.GetValue<string>("FileBaseUrl")!;

        /// <summary>
        /// 날짜변환
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDateFormat(string? date, string type, bool? isDot = false, bool? isFirstYearDelete = false)
		{
			string strFormat = string.Empty;
			string returnDate;
			try
			{
				if (type.Equals("s"))
					strFormat = "yyyy-MM-dd HH:mm:ss";
				else if (type.Equals("m"))
					strFormat = "yyyy-MM-dd HH:mm";
				else if (type.Equals("d"))
					strFormat = "yyyy-MM-dd";
				else if (type.Equals("h"))
					strFormat = "yyyy-MM-dd HH";

				returnDate = DateTime.Parse(Convert.ToString(date)!).ToString(strFormat);

				if ((bool)isDot!) returnDate = returnDate.Replace("-", ".");
				if ((bool)isFirstYearDelete!) returnDate = returnDate[2..];
			}
			catch
			{
				returnDate = date!;
			}
			return returnDate!;
		}

		/// <summary>
		/// GUID 조회
		/// </summary>
		/// <returns></returns>
		public static string GetGuId()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }

        /// <summary>
        /// VB Left 메서드
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
		public static string Left(string value, int count)
        {
            return count > value.Length ? value : value[..count];
        }

        /// <summary>
        /// VB Right 메서드
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
		public static string Right(string value, int count)
        {
            int valueLength = value.Length;

            return count > valueLength ? value : value.Substring(valueLength - count, count);
        }

		/// <summary>
		///	데이터 메모리 할당 크기 구하기
		/// </summary>
		/// <returns></returns>
		public static int GetCapacity(Dictionary<string, string> dataList, bool isArray = false)
		{
			int separatorCount = 8;         // 파라미터당 구분자갯수(보수적으로 크게잡는다) => key쌍따옴표(2) + value쌍따옴표(2) + 콤마(1) + 배열중괄호(2) + 배열콤마(1)
			int capacity = 0;

			foreach (KeyValuePair<string, string> param in dataList)
			{
				capacity += param.Key.Length + separatorCount;
				capacity += param.Value.Length + separatorCount;
			}

			// 배열인 경우 대괄호포함 4를 더한다.
			capacity += isArray ? 4 : 2;

			return capacity;
		}

		/// <summary>
		///	입력여부 확인
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool CheckEmpty(string value)
        {
            bool isCheck = true;

            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    isCheck = false;
                }
            }
            catch
            {
                isCheck = false;
            }

            return isCheck;
        }

        /// <summary>
		///	파라미터 유효성 체크
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool CheckValidation(string key, string value)
        {
            string regex = string.Empty;
            bool isBeforeCheck = false;
            bool status = true;

            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    status = false;
                }
                else
                {
                    int valueLength = value.Length;
                    string firstChar = Left(value, 1);
                    string lastChar = Right(value, 1);

					if (key.Equals("loginId") || key.Equals("memberEmail"))
					{
						regex = @"^[A-Za-z0-9_+\.\-]+@[A-Za-z0-9\-]+\.[A-Za-z0-9\-]+";
					}
                    else if (key.Equals("authSendKey") || key.Equals("authCheckKey"))
                    {
                        regex = @"^[A-Z0-9]{32}$";
                    }
                    else if (key.Equals("authCode"))
                    {
                        regex = @"^[0-9]{4}$";
                    }
                    else if (key.Equals("loginPassword") || key.Equals("memberPassword") || key.Equals("newPassword"))
                    {
                        regex = @"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d!@#$%^&*()_+,-./:;<=>?@[\]^_`{|}~]{6,20}$";
                    }
					else if (key.Equals("memberName"))
					{
						regex = @"^([가-힣]{2,5}|[A-Za-z]+(?:\s[A-Za-z]+){0,2})$";
					}
					else if (key.Equals("koreaHpNumber"))
                    {
                        regex = @"^010\d{7,8}$";
                    }
                    else if (key.Equals("notKoreaHpNumber"))
                    {
                        regex = @"^\d{8,16}$";
                    }
					else if (key.Equals("shippingAddress"))
					{
						isBeforeCheck = true;
						status = valueLength > 0;
					}
					else if (key.Equals("coinAddress"))
                    {
                        regex = @"^0x[a-fA-F0-9]{40}$";
                    }

                    if (status && !isBeforeCheck)
                    {
                        status = regex == null || new Regex(regex)!.IsMatch(value);
                    }
                }
            }
            catch
            {
                status = false;
            }

            return status;
        }

		/// <summary>
		///	유효성 검사 메세지
		/// </summary>
		/// <param name="key"></param>
		/// <param name="checkType"></param>
		/// <returns></returns>
		public static string GetMessage(string key, string checkType)
        {
			string message = string.Empty;
			string title = string.Empty;

			if (key.Equals("loginId")) title = "아이디"; 
			else if (key.Equals("memberEmail")) title = "이메일";
			else if (key.Equals("loginPassword") || key.Equals("memberPassword")) title = "비밀번호";
            else if (key.Equals("authCode")) title = "인증 코드";
            else if (key.Equals("memberName")) title = "이름";
            else if (key.Equals("hpNumber")) title = "휴대폰 번호";
            else if (key.Equals("shippingAddress")) title = "배송지 주소";
            else if (key.Equals("coinAddress")) title = "지갑 주소";

            if (checkType.Equals("0001")) message = "필수 정보 입니다(" + title + ")";
			else if (checkType.Equals("0002")) message = title + "을(를) 바르게 입력하세요.";

			message = string.IsNullOrEmpty(message) ? "잘못된 요청 입니다." : message;

			return message;
		}

        /// <summary>
        /// 한글 마지막 부분 받침 존재 여부 체크
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public static bool GetIsExistLastSyllable(char value)
		{
			int code = Convert.ToInt32(value);
			bool status = code >= 0xAC00 && code <= 0xD7A3;

			return status && (code - 0xAC00) % 28 > 0;
		}

		/// <summary>
		///	XSS 태그 변환
		/// </summary>
		/// <returns></returns>
		public static string SetXSSStringConvert(string value)
		{
			string result = string.Empty;

			if (!string.IsNullOrEmpty(value))
			{
				StringBuilder sb = new();
				sb.Append(HttpUtility.HtmlEncode(value));
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "″");
				sb.Replace("'", "′");
				result = sb.ToString();
			}

			return result;
		}

		/// <summary>
		///	콤마처리
		/// </summary>
		/// <param name="price"></param>
		/// <returns></returns>
		public static string SetComma(string price, bool? isRemoveComma = false)
		{
			int pointCount = 8;
			string result;
			try
			{
				price = Convert.ToDecimal(price).ToString();

				string firstPirce = "0";
				string lastPrice = string.Empty;

				if (price.IndexOf(".") > -1)
				{
					pointCount++;

					string formatPrice = price.Contains('.') ? price.TrimStart('0').TrimEnd('0').TrimEnd('.') : price.TrimStart('0');
					if (!string.IsNullOrEmpty(formatPrice))
					{
						if (formatPrice.IndexOf(".") > -1)
						{
							decimal roundPrice = Math.Round(Convert.ToDecimal(formatPrice), pointCount);

							string[] arrayPrice = roundPrice.ToString().Split('.');
							firstPirce = string.IsNullOrEmpty(arrayPrice[0]) ? "0" : arrayPrice[0];

							if (arrayPrice[1].Length >= pointCount)
							{
								arrayPrice[1] = arrayPrice[1][..(pointCount - 1)];
							}

							lastPrice = "." + arrayPrice[1];

							int cutDecimalCount = 0;
							int loopCount = 0;
							string lastValue = string.Empty;

							// 후행 0이 남아있다면 제거한다.
							while (true)
							{
								lastValue = lastPrice.Substring(lastPrice.Length - (loopCount + 1), 1);
								if (lastValue.Equals("0"))
								{
									cutDecimalCount++;
								}
								else
								{
									break;
								}
								loopCount++;
							}

							lastPrice = lastPrice[..^cutDecimalCount];
						}
						else
						{
							firstPirce = formatPrice;
							lastPrice = string.Empty;
						}
					}
				}
				else
				{
					firstPirce = price;
				}
				result = ((bool)isRemoveComma! ? firstPirce.ToString() : string.Format("{0:#,0}", Convert.ToInt64(firstPirce))) + lastPrice;
				result = firstPirce.Equals("-0") ? "-" + result : result;
			}
			catch
			{
				result = price;
			}

			return result;
		}

		/// <summary>
		/// 이미지 키 URL변환
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="key"></param>
		/// <param name="isVideo"></param>
		/// <returns></returns>
		public static string SetImageUrl(string folder, string key, bool isVideo = false)
        {
            string result = string.Empty;
			string ext = isVideo ? "mp4" : "webp";

            if (!string.IsNullOrEmpty(key))
            {
                result = _fileBaseUrl + folder + "/" + key + "." + ext;
            }

            return result;
        }

        /// <summary>
        /// 파일 키 URL변환
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="key"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string SetFileUrl(string folder, string key, string? ext = "")
		{
			if (string.IsNullOrEmpty(ext))
			{
				ext = "webp";
			}

			string result = string.Empty;
			if (!string.IsNullOrEmpty(key))
			{
				result = _fileBaseUrl + folder + "/" + key + "." + ext;
			}

			return result;
		}

		/// <summary>
		/// 랜덤수 생성
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string GetRandomNumber(int num)
		{
			Random ran = new();
			int numLen = num.ToString().Length - 1;
			string randomNumber = ran.Next(num).ToString();
			randomNumber = randomNumber.PadLeft(numLen, '0');

			return randomNumber;
		}

		/// <summary>
		/// 두 날짜의 차이 구하기
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public static int GetDiffDay(string start, string end)
		{
			return (DateTime.Parse(end) - DateTime.Parse(start)).Days;
		}

		/// <summary>
		/// 숫자 심볼 더하기
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetNumberSymbol(string value)
		{
			int valueLength = value.Length;

			if (valueLength >= 4)
			{
				int[] symbolLengths = { 4, 7, 10, 13, 16, 19, 22, 25 };
				string[] symbols = { "K", "M", "G", "T", "P", "E", "Z", "Y" };

				// 자릿수 체크
				for (int i = 0; i < symbolLengths.Length; i++)
				{
					// 범위 시작 수
					int startLength = symbolLengths[i];
					// 범위 종료 수( 2단위 까진 같은 심볼 )
					int endLength = startLength + 2;

					// 해당 범위 내 있으면 소수점 처리( 1자리 반올림 )
					if (valueLength >= startLength && valueLength <= endLength)
					{
						// 앞자리 값
						string startValue = value[..(valueLength - startLength + 1)];
						int startValueLength = startValue.Length;

						// 뒷자리 값( 앞자리가 1자리 일때만 뒷 소수점 1자리를 붙인다 )
						string endValue = string.Empty;

						if (startValueLength == 1)
						{
							endValue = value.Substring(1, 1);
							endValue = endValue.Equals("0") ? string.Empty : "." + endValue;
						}

						value = startValue + endValue + symbols[i];
						break;
					}
				}
			}

			return value;
		}

		/// <summary>
		/// 바이트만큼 글자수 자르기
		/// </summary>
		/// <param name="value"></param>
		/// <param name="cutCount"></param>
		/// <returns></returns>
		public static string SetCharacterCutByte(string value, int cutCount)
		{
			int byteCount = Encoding.Default.GetByteCount(value);
			string result = value;

			if (byteCount > cutCount)
			{
				byte[] bytes = Encoding.Default.GetBytes(value);
				result = Encoding.Default.GetString(bytes, 0, cutCount) + "...";
			}

			return result;
		}

		/// <summary>
		/// 랜덤 TOKEN ID 조회
		/// </summary>
		/// <param name="nftCount"></param>
		/// <param name="existingTokenIds"></param>
		/// <returns></returns>
		public static int GetRandomTokenId(int nftCount, List<int> existingTokenIds)
		{
			int result = 0;

			if (existingTokenIds != null)
			{
				// 1부터 10000까지의 ID 중에서 발행되지 않은 것들을 추출
				List<int> availableTokenIds = Enumerable.Range(1, nftCount).Except(existingTokenIds).ToList();

				// 모든 토큰 ID가 발행되었는지 확인
				if (availableTokenIds.Count > 0)
				{
					Random random = new();
					int index = random.Next(availableTokenIds.Count);
					result = availableTokenIds[index];
				}
			}

			return result;
		}
	}
}