using System.Data.SqlClient;

namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
    public class Order_Dac
	{
		readonly DBHelper _dbHelper = new();
		readonly Order_Sql _query = new();
        readonly ExceptionLog_Dac _exceptionLogDac = new();

		/// <summary>
		/// 주문 정보 등록 (ORDER_IDX값 반환)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public int Insert_Order(Order_Dto request)
		{
            int result;

            try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@ORDER_NUMBER", request.ORDER_NUMBER),
					new SqlParameter("@PRODUCT_IDX", request.PRODUCT_IDX),
					new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX),
					new SqlParameter("@STATE_CODE", request.STATE_CODE),
					new SqlParameter("@QUANTITY", request.QUANTITY),
					new SqlParameter("@AMOUNT", request.ORDER_AMOUNT),
					new SqlParameter("@SHIPPING_ADDRESS", request.SHIPPING_ADDRESS),
					new SqlParameter("@IS_DEFAULT_ADDRESS", request.IS_DEFAULT_ADDRESS),
					new SqlParameter("@ORDER_TYPE_CODE", request.ORDER_TYPE_CODE),
					new SqlParameter("@DEPOSIT_ACCOUNT_IDX", request.DEPOSIT_ACCOUNT_IDX),
					new SqlParameter("@DEPOSIT_NAME", request.DEPOSIT_NAME),
					new SqlParameter("@BANK_CODE", request.BANK_CODE),
					new SqlParameter("@ACCOUNT_OWNER", request.ACCOUNT_OWNER),
					new SqlParameter("@ACCOUNT_NUMBER", request.ACCOUNT_NUMBER),
					new SqlParameter("@COIN_ADDRESS", request.COIN_ADDRESS),
					new SqlParameter("@TXID", request.TXID),
					new SqlParameter("@TOKEN_ID", request.TOKEN_ID),
					new SqlParameter("@NONCE", request.NONCE)
				};
				if (!string.IsNullOrEmpty(request.PAYMENT_ID)) param.Add(new SqlParameter("@PAYMENT_ID", request.PAYMENT_ID));

				result = Convert.ToInt32(_dbHelper.RunCmdScalar(_query.Insert_Order_Sql(request), param.ToArray()));
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				throw;
			}
			
			return result;
		}

		/// <summary>
		/// 내 주문 목록 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public List<Order_Dto> Select_OrderList(Order_Dto request)
		{
			List<Order_Dto>? list = null;

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PAGE", request.PAGE.HasValue ? request.PAGE : 1),
					new SqlParameter("@PAGE_SIZE", request.PAGE_SIZE.HasValue ? request.PAGE_SIZE : 1),
					new SqlParameter("@MEMBER_IDX", request.MEMBER_IDX),
					new SqlParameter("@START_DAY", request.START_DAY),
					new SqlParameter("@END_DAY", request.END_DAY)
				};
				if (!string.IsNullOrEmpty(request.STATE_CODE)) param.Add(new SqlParameter("@STATE_CODE", request.STATE_CODE));
				if (!string.IsNullOrEmpty(request.PRODUCT_NAME)) param.Add(new SqlParameter("@PRODUCT_NAME", request.PRODUCT_NAME));

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_OrderList_Sql(request), param.ToArray());

				if (reader.HasRows)
				{
					list = new();
					while (reader.Read())
					{
						Order_Dto info = new()
						{
							ORDER_IDX = Convert.ToInt32(reader["ORDER_IDX"]),
							ORDER_NUMBER = reader["ORDER_NUMBER"].ToString(),
							ORDER_TYPE_CODE = reader["ORDER_TYPE_CODE"].ToString(),
							PRODUCT_IDX = Convert.ToInt32(reader["PRODUCT_IDX"]),
							PRODUCT_NAME = reader["PRODUCT_NAME"].ToString(),
							THUMBNAIL_IMAGE_KEY = reader["THUMBNAIL_IMAGE_KEY"].ToString(),
							AMOUNT = Convert.ToInt32(reader["AMOUNT"]),
							INS_DATE = Function.GetDateFormat(reader["INS_DATE"].ToString(), "d"),
							QUANTITY = Convert.ToInt32(reader["QUANTITY"]),
							ORDER_AMOUNT = Convert.ToInt32(reader["ORDER_AMOUNT"]),
							STATE_CODE = reader["STATE_CODE"].ToString(),
							STATE_CODE_NAME = reader["STATE_CODE_NAME"].ToString(),
							COIN_ADDRESS = reader["COIN_ADDRESS"].ToString(),
							TXID = reader["TXID"].ToString(),
							TOKEN_ID = Convert.ToInt32(reader["TOKEN_ID"]),
							NONCE = Convert.ToInt64(reader["NONCE"]),
							TOTAL_COUNT = Convert.ToInt32(reader["TOTAL_COUNT"]),
						};
						info.THUMBNAIL_IMAGE_URL = Function.SetImageUrl("product/thumbnail", info.THUMBNAIL_IMAGE_KEY!);

						list.Add(info);
					}
				}
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return list!;
		}

		/// <summary>
		/// 주문정보 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Order_Dto Select_OrderInfo(Order_Dto request)
		{
			Order_Dto? response = null;

			try
			{
				List<SqlParameter> param = new();
				if (request.ORDER_IDX.HasValue) param.Add(new SqlParameter("@ORDER_IDX", request.ORDER_IDX));
				if (!string.IsNullOrEmpty(request.ORDER_NUMBER)) param.Add(new SqlParameter("@ORDER_NUMBER", request.ORDER_NUMBER));

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_OrderInfo_Sql(request), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						response = new Order_Dto
						{
							ORDER_IDX = Convert.ToInt64(reader["ORDER_IDX"]),
							ORDER_NUMBER = reader["ORDER_NUMBER"].ToString(),
							ORDER_AMOUNT = Convert.ToInt32(reader["ORDER_AMOUNT"]),
							QUANTITY = Convert.ToInt32(reader["QUANTITY"]),
							SHIPPING_ADDRESS = reader["SHIPPING_ADDRESS"].ToString(),
							STATE_CODE = reader["STATE_CODE"].ToString(),
							STATE_CODE_NAME = reader["STATE_CODE_NAME"].ToString(),
							ORDER_TYPE_CODE = reader["ORDER_TYPE_CODE"].ToString(),
							MEMBER_IDX = Convert.ToInt32(reader["MEMBER_IDX"]),
							DEPOSIT_ACCOUNT_IDX = Convert.ToInt32(reader["DEPOSIT_ACCOUNT_IDX"]),
							DEPOSIT_NAME = reader["DEPOSIT_NAME"].ToString(),
							INS_DATE = Function.GetDateFormat(reader["INS_DATE"].ToString(), "s"),
							BANK_CODE = reader["BANK_CODE"].ToString(),
							BANK_NAME = reader["BANK_NAME"].ToString(),
							ACCOUNT_OWNER = reader["ACCOUNT_OWNER"].ToString(),
							ACCOUNT_NUMBER = reader["ACCOUNT_NUMBER"].ToString(),
							MEMBER_KEY = reader["MEMBER_KEY"].ToString(),
							MEMBER_NAME = reader["MEMBER_NAME"].ToString(),
							MEMBER_ID = reader["MEMBER_ID"].ToString(),
							HP_NUMBER = reader["HP_NUMBER"].ToString(),
							PRODUCT_IDX = Convert.ToInt32(reader["PRODUCT_IDX"]),
							PRODUCT_NAME = reader["PRODUCT_NAME"].ToString(),
							THUMBNAIL_IMAGE_KEY = reader["THUMBNAIL_IMAGE_KEY"].ToString(),
							PRODUCT_AMOUNT = Convert.ToInt32(reader["PRODUCT_AMOUNT"]),
							COIN_ADDRESS = reader["COIN_ADDRESS"].ToString(),
							TXID = reader["TXID"].ToString(),
							TOKEN_ID = Convert.ToInt32(reader["TOKEN_ID"]),
							NONCE = Convert.ToInt64(reader["NONCE"])
						};
						response.THUMBNAIL_IMAGE_URL = Function.SetImageUrl("product/thumbnail", response.THUMBNAIL_IMAGE_KEY!);
					}
				}
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
				throw;
			}

			return response!;
		}

		/// <summary>
		/// 민팅 NFT 내역조회
		/// </summary>
		/// <param name="productIdx"></param>
		/// <returns></returns>
		public List<int> Select_MintingNftList(int productIdx)
		{
			List<int>? list = new();

			try
			{
				List<SqlParameter> param = new()
				{
					new SqlParameter("@PRODUCT_IDX", productIdx)
				};

				using SqlDataReader reader = _dbHelper.RunCmd(_dbHelper.Open(), _query.Select_MintingNftList_Sql(), param.ToArray());

				if (reader.HasRows)
				{
					while (reader.Read())
					{
						list.Add(Convert.ToInt32(reader["TOKEN_ID"]));
					}
				}
			}
			catch (Exception ex)
			{
				_exceptionLogDac.Insert_ExceptionLog(ex);
			}

			return list!;
		}
	}
}