using System.Threading.Tasks;
using ParkingLotApi;
using Xunit;

namespace ParkingLotApiTest.ControllerTest
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using ParkingLotApi.Repository;
    using ParkingLotApi.Service;
    using ParkingLotApiTest.Dtos;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Text;

    public class ParkingLotServiceTest : TestBase
    {
        public ParkingLotServiceTest(CustomWebApplicationFactory<Program> factory)
            : base(factory)
        {
        }

        [Fact]
        public async Task Should_create_parkingLot_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            // when
            await parkingLotService.AddParkingLot(TestData.ParkingLotDtos[0]);

            // then
            Assert.Equal(1, context.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_get_parkingLot_byId_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            var id = await parkingLotService.AddParkingLot(TestData.ParkingLotDtos[0]);
            // when
            var targetParkingLotDto = await parkingLotService.GetById(id);

            // then
            Assert.Equal("park1", targetParkingLotDto.Name);
        }

        [Fact]
        public async Task Should_get_all_parkingLots_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            TestData.ParkingLotDtos.ForEach(async d => await parkingLotService.AddParkingLot(d));
            //when
            List<ParkingLotDto> targetParkingLots = await parkingLotService.GetAll();
            //then
            Assert.Equal("park1", targetParkingLots[0].Name);
            Assert.Equal(4, targetParkingLots.Count);
        }

        [Fact]
        public async Task Should_delete_a_parkingLot_by_id_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            var id = await parkingLotService.AddParkingLot(TestData.ParkingLotDtos[0]);
            //when
            await parkingLotService.deleteParkingLot(id);
            //then
            Assert.Equal(0, context.ParkingLots.Count());
        }

        [Fact]
        public async Task Should_get_parkingLots_by_pageIndex_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            TestData.ParkingLotDtos.ForEach(async parkingLotDto => await parkingLotService.AddParkingLot(parkingLotDto));
            //when
            var targetParkingLots = await parkingLotService.GetByPageIndex(1);
            //then
            Assert.Equal(4, targetParkingLots.Count());
        }

        [Fact]
        public async Task Should_update_a_parkingLot_Capacity_by_Id_success_via_parkingLot_service()
        {
            // given
            var context = GetParkingLotDbContext();
            IParkingLotService parkingLotService = new ParkingLotService(context);
            var id = await parkingLotService.AddParkingLot(TestData.ParkingLotDtos[1]);
            TestData.ParkingLotDtos[1].Capacity = 30;
            //when
            var targetParkingLot = await parkingLotService.UpdateParkingLotCapacity(id, TestData.ParkingLotDtos[1]);
            //then
            Assert.Equal(30, targetParkingLot.Capacity);
        }

        private ParkingLotContext GetParkingLotDbContext()
        {
            var scope = Factory.Services.CreateScope();
            var scopedService = scope.ServiceProvider;
            ParkingLotContext context = scopedService.GetRequiredService<ParkingLotContext>();
            return context;

        }
    }
}