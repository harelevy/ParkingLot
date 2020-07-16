using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParkingLotProject.Logic
{
    public class LogicManager
    {
        public string ImagePath { get; set; }
        public List<VehicleModel> Vehicles { get; set; }

        public LogicManager()
        {
            Vehicles = new List<VehicleModel>();
        }

        internal async Task<string> ExtractIdNumberFromImage()
        {
            string result = "";
            if (string.IsNullOrEmpty(ImagePath))
                throw new Exception("Error! invalid image path.\nPlease upload an image first.");
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.Timeout = new TimeSpan(1, 1, 1);


                MultipartFormDataContent form = new MultipartFormDataContent();
                form.Add(new StringContent("c46b3fb6c188957"), "apikey"); //Added api key in form data
                form.Add(new StringContent("eng"), "language");

                form.Add(new StringContent("2"), "ocrengine");
                form.Add(new StringContent("true"), "scale");
                form.Add(new StringContent("true"), "istable");

                if (string.IsNullOrEmpty(ImagePath) == false)
                {
                    byte[] imageData = File.ReadAllBytes(ImagePath);
                    form.Add(new ByteArrayContent(imageData, 0, imageData.Length), "image", "image.jpg");
                }

                HttpResponseMessage response = await httpClient.PostAsync("https://api.ocr.space/Parse/Image", form);
                string strContent = await response.Content.ReadAsStringAsync();
                OcrResult ocrResult = JsonConvert.DeserializeObject<OcrResult>(strContent);


                if (ocrResult.OCRExitCode == 1)
                {
                    result = ocrResult.ParsedResults[0].ParsedText;
                    result = result.Substring(0, result.IndexOf("\n") - 1);
                    Regex regex = new Regex("[^a-zA-Z0-9]");
                    result = regex.Replace(result, "");
                }
                else
                {
                    throw new Exception("Error connecting the API.");
                }

                return result;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        internal int MakeDecision(string i_id)
        {
            int isAllowed = 1;
            string twoLastDigits = i_id.Substring(i_id.Length - 2); // asumption that id length > 1
            if (isPublicTransportation(twoLastDigits) || isMilitary(i_id) || ruleC(i_id, twoLastDigits) || operatedByGas(i_id))
            {
                //(rule C - e.g 7 digit number ending with 85/86/87/88/89/00)
                isAllowed = 0;
            }
            return isAllowed;

        }

        private bool isPublicTransportation(string twoLastDigits)
        {
            return twoLastDigits == "25" || twoLastDigits == "26" ? true : false;
        }

        private bool operatedByGas(string i_id)
        {
            bool result = false;
            int id = int.Parse(i_id);
            int sum = 0;
            if (i_id.Length == 7 || i_id.Length == 8)
            {
                while (id > 0)
                {
                    sum += id % 10;
                    id = id / 10;
                }
                if (sum % 7 == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ruleC(string i_id, string i_twoLastDigits)
        {
            int twoLastDigits = int.Parse(i_twoLastDigits); // gurentee there is no Alphabetic charecters
            if (i_id.Length == 7)
            {
                return ((twoLastDigits >= 85 && twoLastDigits <= 89) || twoLastDigits == 00);
            }
            else return false;
        }

        private bool isMilitary(string i_id)
        {
            bool result = false;
            for (int i = 0; i < i_id.Length; i++)
            {
                if (char.IsLetter(i_id[i]))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }


    }
}
