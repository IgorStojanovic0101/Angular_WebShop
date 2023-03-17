using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.NET
{
    public class ConsumeProductModel
    {
      
        private static string trainerModelFile;
        // For more info on consuming ML.NET models, visit https://aka.ms/mlnet-consume
        // Method for consuming model in your app

        public  ReturnModel Predict(ModelInput input)
        {
           // trainerModelFile = @"D:\Projects\Diplomski\Api i klijent\Files\Product_trainingModel_User_" + input.UserId.ToString()+".zip";
            trainerModelFile = @"D:\Projects\Diplomski\Api i klijent\Files\Product_trainingModel_User.zip";

            ReturnModel returnModel = new ReturnModel();
            if (File.Exists(trainerModelFile))
            {
                returnModel.valid = true;
                returnModel.output = PredictionEngine.Value.Predict(input);
            }
            return returnModel;
        }      
        private  Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictionEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(CreatePredictionEngine);

       
        public static PredictionEngine<ModelInput, ModelOutput> CreatePredictionEngine()
        {
            MLContext mlContext = new MLContext();
     
            ITransformer mlModel = mlContext.Model.Load(trainerModelFile, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            return predEngine;
        }
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
