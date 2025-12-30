using Microsoft.AspNetCore.Mvc;

namespace Broker_Projekt_Zaliczeniowy.ModelsDto
{
    public class AddAssetDto 
    {
        public string Ticker { get; set; } = string.Empty;
         public string FullName { get; set; } = string.Empty;

    }
}
