﻿using CardsServer.BLL.Dto.Card;
using CardsServer.BLL.Dto.Statistic;
using CardsServer.BLL.Infrastructure.Auth;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StatisticService.API;


namespace CardsServer.API.Controllers
{
    [ApiController]
    [Authorize]
    public class StatisticController : ControllerBase
    {
        private readonly BLL.Services.gRPC.StatisticService _service;

        public StatisticController(BLL.Services.gRPC.StatisticService service)
        {
            _service = service;
        }

        /// <summary>
        /// Проверка соединения с gRPC микросервисом
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Ping")]
        public async Task<IActionResult> Ping()
        {
            PingResponse result;
            try
            {
                result = await _service.PingAsync(new PingRequest());
            }
            catch (Exception ex)
            {
                return BadRequest("Связаться не получилось((");
            }
            return Ok(result);

        }

        /// <summary>
        /// Сохраняет статистику модуля
        /// </summary>
        /// <param name="moduleStatistic"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("statistic")]
        public async Task<IActionResult> SaveModuleStatistic(
            SaveModuleStatistic moduleStatistic, CancellationToken cancellationToken)
        {
            int userId = AuthExtension.GetId(User);

            Timestamp timeNowInTimestampFormat = DateTime.UtcNow.ToTimestamp();

            // Создаем модель для пересылки
            StatisticRequest requestModel = new()
            {
                CompletedAt = timeNowInTimestampFormat,
                UserId = userId,
                ModuleId = moduleStatistic.ModuleId
            };

            // Т.к в protobuf у нас repeated - то наши поля автоматически read-only.
            // То есть инициализировать мы их не можем((
            // Приходится добавлять после инициализации объекта
            if (!moduleStatistic.ElementStatistics.IsNullOrEmpty())
            {
                requestModel.Elements.AddRange(
                    moduleStatistic.ElementStatistics.Select(y => new StatisticElements()
                {
                    Answer = y.Answer,
                    ElementId = y.ElementId
                }));
            }
            StatisticResponse result = await _service
                .SaveStatisticAsync(requestModel);

            return Ok(result);
        }

        [HttpGet("statistic")]
        public async Task<IActionResult> GetStatisticById(int id)
        {
            GetStatisticByIdResponse response = await _service
                .GetStatisticByIdAsync(new GetStatisticByIdRequest { Id = id });

            GetStatisticDto result = new()
            {
                CompletedAt = response.CompletedAt.ToDateTime(),
                NumberOfAttempts = response.NumberOfAttempts,
                PercentSuccess = response.PercentSuccess,
            };

            return Ok(result);
        }


        /// <summary>
        /// Возвращает статистику по действиям юзера в год 
        /// TODO - вынести в отдельный микросервис (gRPC)
        /// </summary>
        /// <returns></returns>
        [HttpGet("statistic/year")]
        public async Task<IActionResult> GetYearStatisic(int year)
        {
            // Пока мокаю
            //YearStatisticData[][] res = GenerateYearStatistic(2024);

            //List<int> colspan = GenerateColspan(res[6], year);

            int userId = AuthExtension.GetId(User);

            YearStatisticResponse responseFromGrpcService = await _service
                .GetYearStatisicAsync(new()
            {
                UserId = userId,
                Year = year
            });

            YearStatisticDto result = new()
            {
                Year = year,
                Colspan = [.. responseFromGrpcService.Colspan],
                Data = ConvertRepeatedField(responseFromGrpcService.Data)
            };

            return Ok(result);

        }

        /// <summary>
        /// Позволяет получить список доступных годов статистики
        /// </summary>
        /// <returns></returns>
        [HttpGet("statistic/available-years")]
        public async Task<IActionResult> GetAvailableYears(CancellationToken cancellationToken)
        {
            int userId = AuthExtension.GetId(User);
            return Ok();
        }


        ///// <summary>
        ///// TODO: Позволяет получить все объекты статистики по данному модулю, ассоциированную с пользователем
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("statistic/{id}")]
        //public async Task<IActionResult> GetAllStatisticForModuleByUser()
        //{
        //    return Ok();
        //}


        /// <summary>
        /// TODO: Получает статистику по определенному модулю
        /// Вероятно надо добавить еще либо дату, либо какой-то уточняющий фактор для получения статистики
        /// </summary>
        /// <returns></returns>
        //[HttpGet("statistic/{id}")]
        //public async Task<IActionResult> GetModuleStatistic(
        //    int id)
        //{
        //    return Ok();
        //}

        ///// <summary>
        ///// Получает статистику по всем модулям, ассоциированым с юзером
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("statistic")]
        //public async Task<IActionResult> GetModulesStatistic(
        //    )
        //{
        //    return Ok();
        //}
        private static YearStatisticData[][] ConvertRepeatedField(RepeatedField<YearStatisticRow> protobufField)
        {
            return protobufField
                .Select(row => row.Values
                    .Select(value => new YearStatisticData
                    {
                        Date = value.Date.ToDateTime(),
                        Value = value.Value
                    })
                    .ToArray())
                .ToArray();
        }

        /// <summary>
        /// Получение данный об активности пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("statistic/last-activity")]
        public async Task<IActionResult> GetLastActivity()
        {
            int userId = AuthExtension.GetId(User);
            // Пока мокаем
            LastActivityDTO data = new()
            {
                ActivityList = [
                    new LastActivityModel()
                    {
                        Id = 15,
                        Name = "Билибоба"
                    },
                    new LastActivityModel()
                    {
                        Id = 16,
                        Name = "Боба"
                    },
                    new LastActivityModel()
                    {
                        Id = 17,
                        Name = "билиб"
                    }
                    ]
            };

            return Ok(data);
        }
    }
}
