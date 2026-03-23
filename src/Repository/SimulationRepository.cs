using api_financiamento.src.Configuration;
using api_financiamento.src.Interfaces;
using api_financiamento.src.Models;
using api_financiamento.src.Models.Base;
using api_financiamento.src.Shared.Utils;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace api_financiamento.src.Repository
{
    public class SimulationRepository(AppDbContext context) : ISimulationRepository
    {
        #region CREATE
        public async Task<ResponseApi<Simulation?>> CreateAsync(Simulation simulation)
        {
            try
            {
                await context.Simulations.InsertOneAsync(simulation);
                return new(simulation, 201, "Simulação criada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region READ
        public async Task<ResponseApi<List<dynamic>>> GetAllAsync(PaginationUtil<Simulation> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$sort", pagination.PipelineSort),
                    new("$skip", pagination.Skip),
                    new("$limit", pagination.Limit),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                        {"id", new BsonDocument("$toString", "$_id")},
                        {"vehicleName", 1},
                        {"clientName", 1},
                        {"vehicleValue", 1},
                        {"downPayment", 1},
                        {"monthlyInterestRate", 1},
                        {"installments", 1},
                        {"financedAmount", 1},
                        {"installmentValue", 1},
                        {"totalPaid", 1},
                        {"totalInterest", 1},
                        {"createdAt", 1},
                    }),
                    new("$sort", pagination.PipelineSort),
                };

                List<BsonDocument> results = await context.Simulations.Aggregate<BsonDocument>(pipeline).ToListAsync();
                List<dynamic> list = results.Select(doc => BsonSerializer.Deserialize<dynamic>(doc)).ToList();
                return new(list);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                BsonDocument[] pipeline = [
                    new("$match", new BsonDocument{
                        {"_id", new ObjectId(id)},
                        {"deleted", false}
                    }),
                    new("$addFields", new BsonDocument {
                        {"id", new BsonDocument("$toString", "$_id")},
                    }),
                    new("$project", new BsonDocument
                    {
                        {"_id", 0},
                    }),
                ];

                List<BsonDocument> results = await context.Simulations.Aggregate<BsonDocument>(pipeline).ToListAsync();
                if (results.Count == 0) return new(null, 404, "Simulação não encontrada");

                dynamic result = BsonSerializer.Deserialize<dynamic>(results[0]);
                return new(result);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<ResponseApi<Simulation?>> GetByIdAsync(string id)
        {
            try
            {
                var simulation = await context.Simulations.Find(s => s.Id == id && !s.Deleted).FirstOrDefaultAsync();
                if (simulation is null) return new(null, 404, "Simulação não encontrada");
                return new(simulation);
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }

        public async Task<int> GetCountDocumentsAsync(PaginationUtil<Simulation> pagination)
        {
            try
            {
                List<BsonDocument> pipeline = new()
                {
                    new("$match", pagination.PipelineFilter),
                    new("$count", "total")
                };

                var result = await context.Simulations.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
                return result != null ? result["total"].AsInt32 : 0;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<Simulation?>> UpdateAsync(Simulation simulation)
        {
            try
            {
                var filter = Builders<Simulation>.Filter.Eq(s => s.Id, simulation.Id);
                var update = Builders<Simulation>.Update
                    .Set(s => s.VehicleValue, simulation.VehicleValue)
                    .Set(s => s.DownPayment, simulation.DownPayment)
                    .Set(s => s.MonthlyInterestRate, simulation.MonthlyInterestRate)
                    .Set(s => s.Installments, simulation.Installments)
                    .Set(s => s.FinancedAmount, simulation.FinancedAmount)
                    .Set(s => s.InstallmentValue, simulation.InstallmentValue)
                    .Set(s => s.TotalPaid, simulation.TotalPaid)
                    .Set(s => s.TotalInterest, simulation.TotalInterest)
                    .Set(s => s.VehicleName, simulation.VehicleName)
                    .Set(s => s.ClientName, simulation.ClientName)
                    .Set(s => s.AmortizationTable, simulation.AmortizationTable)
                    .Set(s => s.UpdatedAt, DateTime.UtcNow)
                    .Set(s => s.UpdatedBy, simulation.UpdatedBy);

                await context.Simulations.UpdateOneAsync(filter, update);
                return new(simulation, 200, "Simulação atualizada com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region DELETE
        public async Task<ResponseApi<Simulation>> DeleteAsync(string id)
        {
            try
            {
                var filter = Builders<Simulation>.Filter.Eq(s => s.Id, id);
                var update = Builders<Simulation>.Update
                    .Set(s => s.Deleted, true)
                    .Set(s => s.DeletedAt, DateTime.UtcNow);

                await context.Simulations.UpdateOneAsync(filter, update);
                return new(null!, 200, "Simulação excluída com sucesso");
            }
            catch
            {
                return new(null!, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion
    }
}
