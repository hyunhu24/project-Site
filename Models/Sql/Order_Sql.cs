namespace BRILLIANT_NFT_MARKET_FRONT.Models
{
	public class Order_Sql
	{
		/// <summary>
		/// 주문 정보 등록
		/// </summary>
		/// <returns></returns>
		public string Insert_Order_Sql(Order_Dto request)
		{
			string addColumnQuaery = @"";
			string addValueQuaery = @"";

			if (!string.IsNullOrEmpty(request.PAYMENT_ID)) 
			{
				addColumnQuaery += ", PAYMENT_ID";
				addValueQuaery += ", @PAYMENT_ID";
			}
			string query = @"
				INSERT INTO ORDER_INFO
				(
					ORDER_NUMBER,
					PRODUCT_IDX,
					MEMBER_IDX,
					STATE_CODE,
					QUANTITY,
					AMOUNT,
					SHIPPING_ADDRESS,
					IS_DEFAULT_ADDRESS,
					ORDER_TYPE_CODE,
					DEPOSIT_ACCOUNT_IDX,
					DEPOSIT_NAME,
					BANK_CODE,
					ACCOUNT_OWNER,
					ACCOUNT_NUMBER,
					COIN_ADDRESS,
					TXID,
					TOKEN_ID,
					NONCE
			";
			query += addColumnQuaery;
			query += @"
				)
				VALUES
				(
					@ORDER_NUMBER,
					@PRODUCT_IDX,
					@MEMBER_IDX,
					@STATE_CODE,
					@QUANTITY,
					@AMOUNT,
					@SHIPPING_ADDRESS,
					@IS_DEFAULT_ADDRESS,
					@ORDER_TYPE_CODE,
					@DEPOSIT_ACCOUNT_IDX,
					@DEPOSIT_NAME,
					@BANK_CODE,
					@ACCOUNT_OWNER,
					@ACCOUNT_NUMBER,
					@COIN_ADDRESS,
					@TXID,
					@TOKEN_ID,
					@NONCE
			";
			query += addValueQuaery;
			query += @"
				);

				SELECT SCOPE_IDENTITY() AS ORDER_IDX
			";

			return query;
		}

		/// <summary>
		/// 내 주문 목록 조회
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public string Select_OrderList_Sql(Order_Dto request)
		{
			string addQuery = @"";
			if (!string.IsNullOrEmpty(request.STATE_CODE)) addQuery += " AND ORD.STATE_CODE = @STATE_CODE ";
			if (!string.IsNullOrEmpty(request.PRODUCT_NAME)) addQuery += " AND PROD.PRODUCT_NAME LIKE @PRODUCT_NAME ";

			string query = @"
				WITH LIST AS
				(
					SELECT
						ORD.ORDER_IDX,
						ORD.ORDER_NUMBER,
						ORD.ORDER_TYPE_CODE,
						PROD.PRODUCT_IDX,
						PROD.PRODUCT_NAME,
						PROD.THUMBNAIL_IMAGE_KEY,
						PROD.AMOUNT,
						ORD.INS_DATE,
						ORD.QUANTITY,
						ORD.AMOUNT AS ORDER_AMOUNT,
						ORD.STATE_CODE,
						CODE.MIDDLE_CODE_NAME AS STATE_CODE_NAME,
						ORD.COIN_ADDRESS,
						ORD.TXID,
						ORD.TOKEN_ID,
						ORD.NONCE,
						ROW_NUMBER() OVER(ORDER BY ORD.INS_DATE DESC) AS ROW_NUM
					FROM
						ORDER_INFO AS ORD (NOLOCK)
					INNER JOIN
						PRODUCT_INFO AS PROD (NOLOCK)
							ON PROD.IS_DEL = 0
							AND ORD.PRODUCT_IDX = PROD.PRODUCT_IDX
					INNER JOIN 
						COMMON_CODE_INFO AS CODE (NOLOCK)
							ON CODE.MAIN_CODE = '0004'
							AND ORD.STATE_CODE = CODE.MIDDLE_CODE
					WHERE 
							ORD.MEMBER_IDX = @MEMBER_IDX
						AND	ORD.INS_DATE >= @START_DAY
						AND	ORD.INS_DATE < @END_DAY
			";
			query += addQuery;
			query += @"
				)
				SELECT
					*,
					(SELECT COUNT(*) FROM LIST WITH(NOLOCK)) AS TOTAL_COUNT
				FROM
					LIST WITH (NOLOCK)
				WHERE
					LIST.ROW_NUM BETWEEN (@PAGE - 1) * @PAGE_SIZE + 1 AND @PAGE * @PAGE_SIZE
				ORDER BY
					LIST.ROW_NUM
				ASC
			";

			return query;
		}

		/// <summary>
		/// 주문정보 조회 (무통장입금 / Paypal)
		/// </summary>
		/// <returns></returns>
		public string Select_OrderInfo_Sql(Order_Dto request)
		{
			string whereQuery = @"";

			if (request.ORDER_IDX.HasValue) whereQuery += " AND ORD.ORDER_IDX = @ORDER_IDX ";
			if (!string.IsNullOrEmpty(request.ORDER_NUMBER)) whereQuery += " AND ORD.ORDER_NUMBER = @ORDER_NUMBER ";

			string query = @"
				SELECT
					ORD.ORDER_IDX,
					ORD.ORDER_NUMBER,
					ORD.AMOUNT AS ORDER_AMOUNT,
					ORD.QUANTITY,
					ORD.SHIPPING_ADDRESS,
					ORD.STATE_CODE,
					CODE1.MIDDLE_CODE_NAME AS STATE_CODE_NAME,
					ORD.ORDER_TYPE_CODE,
					ORD.MEMBER_IDX,
					ORD.DEPOSIT_ACCOUNT_IDX,
					ORD.DEPOSIT_NAME,
					ORD.INS_DATE,
					ORD.BANK_CODE,
					CODE2.MIDDLE_CODE_NAME AS BANK_NAME,
					ORD.ACCOUNT_OWNER,
					ORD.ACCOUNT_NUMBER,
					MEM.MEMBER_KEY,
					MEM.MEMBER_NAME,
					MEM.MEMBER_ID,
					MEM.HP_NUMBER,
					PROD.PRODUCT_IDX,
					PROD.PRODUCT_NAME,
					PROD.THUMBNAIL_IMAGE_KEY,
					PROD.AMOUNT AS PRODUCT_AMOUNT,
					ORD.COIN_ADDRESS,
					ORD.TXID,
					ORD.TOKEN_ID,
					ORD.NONCE
				FROM
					ORDER_INFO AS ORD (NOLOCK)
				INNER JOIN 
					MEMBER_INFO AS MEM (NOLOCK)
						ON MEM.IS_DEL = 0
						AND ORD.MEMBER_IDX = MEM.MEMBER_IDX
				INNER JOIN 
					PRODUCT_INFO PROD (NOLOCK)
						ON ORD.PRODUCT_IDX = PROD.PRODUCT_IDX
				INNER JOIN 
					COMMON_CODE_INFO AS CODE1 (NOLOCK)
						ON CODE1.MAIN_CODE = '0004'
						AND ORD.STATE_CODE = CODE1.MIDDLE_CODE
				LEFT JOIN
					COMMON_CODE_INFO AS CODE2 (NOLOCK)
						ON CODE2.MAIN_CODE = '0001'
						AND ORD.BANK_CODE = CODE2.MIDDLE_CODE
				WHERE 
						ORD.IS_DEL = 0
			";
			query += whereQuery;

			return query;
		}

		/// <summary>
		/// 민팅 NFT 내역조회
		/// </summary>
		/// <returns></returns>
		public string Select_MintingNftList_Sql()
		{
			string query = @"
				SELECT
					TOKEN_ID
				FROM
					ORDER_INFO (NOLOCK)
				WHERE 
						TOKEN_ID > 0
					AND PRODUCT_IDX = @PRODUCT_IDX
				ORDER BY
					TOKEN_ID
				ASC
			";

			return query;
		}
	}
}