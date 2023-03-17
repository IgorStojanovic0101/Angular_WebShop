using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.NET
{
    public class ConsumeCategoryModel
    {
        private Lazy<PredictionEngine<CategoryModelInput, ModelOutput>> PredictionEngine = new Lazy<PredictionEngine<CategoryModelInput, ModelOutput>>(CreatePredictionEngine);

        private static string trainerModelFile;
        // For more info on consuming ML.NET models, visit https://aka.ms/mlnet-consume
        // Method for consuming model in your app

        public ReturnModel PredictDatabase(CategoryModelInput input)
        {
           // trainerModelFile = @"D:\Projects\Diplomski\Api i klijent\Files\Category_trainingModel_User_" + input.UserId.ToString() + ".zip";
            trainerModelFile = @"D:\Projects\Diplomski\Api i klijent\Files\Category_trainingModel_User.zip";

            ReturnModel returnModel = new ReturnModel();
            if (File.Exists(trainerModelFile))
            {
                returnModel.valid = true;
                returnModel.output = PredictionEngine.Value.Predict(input);
            }
            return returnModel;
        }

        public static PredictionEngine<CategoryModelInput, ModelOutput> CreatePredictionEngine()
        {
            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine
            //   string modelPath = @"D:\Fakultet\ML.NET\ASP.NET\Api\MLModel2.zip";            
            ITransformer mlModel = mlContext.Model.Load(trainerModelFile, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<CategoryModelInput, ModelOutput>(mlModel);

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
