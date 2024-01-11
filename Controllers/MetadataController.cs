using Microsoft.AspNetCore.Mvc;
using BRILLIANT_NFT_MARKET_FRONT.Models;
using Newtonsoft.Json;
using System.Dynamic;

namespace BRILLIANT_NFT_MARKET_FRONT.Controllers
{
    public class MetadataController : Controller
    {
        [Route("nft/metadata/{contractAddress}/{tokenId}")]
		public IActionResult GetMetadata(string contractAddress, string tokenId)
		{
			bool status;
			dynamic responseData = new ExpandoObject();

			try
			{
				tokenId = tokenId.Replace(".json", "");
				Nft_Dto nftInfo = new Nft_Dac().Select_NftMetadataInfo(contractAddress, Convert.ToInt32(tokenId));

				status = nftInfo != null;

				if (status)
				{
					string externalLink = nftInfo!.EXTERNAL_LINK!;
					string nftPrefixName = nftInfo!.NFT_PREFIX_NAME!;
					string nftName = !string.IsNullOrEmpty(nftPrefixName) ? nftPrefixName + " #" + tokenId : "#" + tokenId;

					responseData.name = nftName;
					responseData.description = nftInfo.NFT_DESCRIPTION;
					responseData.image = nftInfo.IMAGE_URL;

					if (!string.IsNullOrEmpty(externalLink))
					{
						responseData.external_link = externalLink;
					}
				}
			}
			catch
			{
				status = false;
			}

			return new ContentResult
			{
				Content = status ? JsonConvert.SerializeObject(responseData) : "NotFound",
				ContentType = status ? "application/json" : "text/plain",
				StatusCode = 200
			};
		}
    }
}