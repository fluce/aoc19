using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VisualizationLib;

namespace VisualizationServer.Services
{
    public class VisualizationService: VisualizationLib.Visualization.VisualizationBase
    {
        public ILogger<VisualizationService> Logger { get; }

        public VisualizationService(ILogger<VisualizationService> logger)
        {
            Logger = logger;
        }

        public override async Task<Empty> SendScreen(ScreenMessage request, ServerCallContext context)
        {
            Logger.LogInformation($"SendScreen : {request}");
            return new Empty();
        }

        public override async Task<Empty> SendIntCodeTrace(IntCodeTrace request, ServerCallContext context)
        {
            Logger.LogInformation($"SendIntCodeTrace : {request}");
            return new Empty();
        }
    }
}
