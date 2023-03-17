using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ML.NET
{
    public class ModelBuilder
    {
    

        private static MLContext MLcontext = new MLContext(seed: 1);

        public ModelBuilder()
        {

        }

        
        public void UpdateCategory(SendModel model)
        {



           //string FilePath = @"D:\Projects\Diplomski\Api i klijent\Files\Category_trainingModel_User_" + model.userId.ToString() + ".zip";
            string FilePath = @"D:\Projects\Diplomski\Api i klijent\Files\Category_trainingModel_User.zip";

            var ColumnNames = model.tip.GetProperties().Where(x => !x.Name.Contains("_pk") && !x.Name.Contains("tb_"))
                        .Select(property => property.Name)
                        .ToList();

            var loaderColumns = new DatabaseLoader.Column[3];
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                if (ColumnNames[i].Contains("fk"))
                {
                    if (ColumnNames[i].Contains("user_fk"))
                        model.user_name = ColumnNames[i];
                    else
                        model.item_name = ColumnNames[i];

                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Int32 };

                }
                else
                {
                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Single };
                    model.label_name = ColumnNames[i];
                }

            }

            var loader = MLcontext.Data.CreateDatabaseLoader(loaderColumns);

            var factory = DbProviderFactories.GetFactory(model.dbConnection);



            var dbSource = new DatabaseSource(factory, model.conString, "SELECT * FROM " + model.tip.Name);


            IDataView data = loader.Load(dbSource);

            var preview = data.Preview();


            var trainingPipeline = BuildTrainingPipeline(MLcontext, model);

            // Train Model
            var mlModel = TrainModel(MLcontext, data, trainingPipeline);

            //Prediction
            EvaluateModel(MLcontext, data, mlModel, model);


            // Save model
            SaveModel(MLcontext, mlModel, FilePath, data.Schema);


        }
        public void UpdateDepartment(SendModel model)
        {



            //string FilePath = @"D:\Projects\Diplomski\Api i klijent\Files\Department_trainingModel_User_" + model.userId.ToString() + ".zip";
            string FilePath = @"D:\Projects\Diplomski\Api i klijent\Files\Department_trainingModel_User.zip";

            var ColumnNames = model.tip.GetProperties().Where(x => !x.Name.Contains("_pk") && !x.Name.Contains("tb_"))
                        .Select(property => property.Name)
                        .ToList();

            var loaderColumns = new DatabaseLoader.Column[3];
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                if (ColumnNames[i].Contains("fk"))
                {
                    if(ColumnNames[i].Contains("user_fk"))
                        model.user_name = ColumnNames[i];
                    else
                        model.item_name = ColumnNames[i];

                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Int32 };

                }
                else
                {
                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Single };
                    model.label_name = ColumnNames[i];
                }

            }

            var loader = MLcontext.Data.CreateDatabaseLoader(loaderColumns);

            var factory = DbProviderFactories.GetFactory(model.dbConnection);



            var dbSource = new DatabaseSource(factory, model.conString, "SELECT * FROM "+model.tip.Name);


            IDataView data = loader.Load(dbSource);

   
 
            var trainingPipeline = BuildTrainingPipeline(MLcontext, model);

            // Train Model
            var mlModel = TrainModel(MLcontext, data, trainingPipeline);

        

            // Save model
            SaveModel(MLcontext, mlModel, FilePath, data.Schema);


        }
        public void UpdateRating(SendModel model)
        {
            string FilePath = @"D:\Projects\Diplomski\Api i klijent\Files\Product_trainingModel_User.zip";

            var ColumnNames = model.tip.GetProperties().Where(x => !x.Name.Contains("_pk") && !x.Name.Contains("tb_"))
                         .Select(property => property.Name)
                         .ToList();

            var loaderColumns = new DatabaseLoader.Column[3];
            for (int i = 0; i < ColumnNames.Count; i++)
            {
                if (ColumnNames[i].Contains("fk"))
                {
                    if (ColumnNames[i].Contains("user_fk"))
                        model.user_name = ColumnNames[i];
                    else
                        model.item_name = ColumnNames[i];

                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Int32 };

                }
                else
                {
                    loaderColumns[i] = new DatabaseLoader.Column() { Name = ColumnNames[i], Type = DbType.Single };
                    model.label_name = ColumnNames[i];
                }

            }

            var loader = MLcontext.Data.CreateDatabaseLoader(loaderColumns);
            var factory = DbProviderFactories.GetFactory(model.dbConnection);
            var dbSource = new DatabaseSource(factory, model.conString, "SELECT * FROM "+model.tip.Name);

            IDataView data = loader.Load(dbSource);

            var trainingPipeline = BuildTrainingPipeline(MLcontext,model);

            // Train Model
            var mlModel = TrainModel(MLcontext, data, trainingPipeline);
 
            // Save model
            SaveModel(MLcontext, mlModel, FilePath, data.Schema);
        }

        public static IEstimator<ITransformer> BuildTrainingPipelineFieldAware(MLContext mlContext, SendModel model)
        {

            // Define data preparation estimator
            // IEstimator<ITransformer> dataPrepEstimator = mlContext.Transforms.Conversion.MapValueToKey("userId", "userId")
            //  .Append(mlContext.Transforms.Conversion.MapValueToKey("productId", "productId"));
            var dataProcessPipeline = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "userIdFeaturized", inputColumnName: model.user_name)
                                   .Append(mlContext.Transforms.Text.FeaturizeText(outputColumnName: "itemIdFeaturized", inputColumnName: model.item_name))
                                   .Append(mlContext.Transforms.Concatenate("Features", "userIdFeaturized", "itemIdFeaturized"));

            var trainingPipeline = dataProcessPipeline.Append(mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine(labelColumnName:model.label_name,featureColumnName: "Features" ));
            // Create data preparation transformer

            return trainingPipeline;


        }




        public static IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext,SendModel model)
        {
          
            var dataProcessPipeline = mlContext.Transforms.Conversion.MapValueToKey(model.user_name, model.user_name)
                                      .Append(mlContext.Transforms.Conversion.MapValueToKey(model.item_name, model.item_name));
            // Algoritam za treniranje
            var options = new MatrixFactorizationTrainer.Options()
            {
                NumberOfIterations = 100,
                LearningRate = 0.05f,
                ApproximationRank = 128,
                Lambda = 0.05f,
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossRegression,
                Alpha = 0.01f,
                C = 0.01f,
                LabelColumnName = model.label_name,
                MatrixColumnIndexColumnName = model.user_name,
                MatrixRowIndexColumnName = model.item_name
            };
            var trainer = mlContext.Recommendation().Trainers.MatrixFactorization(options);

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            return trainingPipeline;
        }

        void EvaluateModel(MLContext mlContext, IDataView testDataView, ITransformer model,SendModel sendmodel)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);
            var matrix = MLcontext.Recommendation().Evaluate(data: prediction, labelColumnName: sendmodel.label_name);
            var metrics = MLcontext.Regression.Evaluate(prediction, labelColumnName: sendmodel.label_name, scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }

        public static ITransformer TrainModel(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            var transormer = trainingPipeline.Fit(trainingDataView);

            return transormer;

        }



        private static void SaveModel(MLContext mlContext, ITransformer mlModel, string modelRelativePath, DataViewSchema modelInputSchema)
        {
        
            mlContext.Model.Save(mlModel, modelInputSchema, modelRelativePath);

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
