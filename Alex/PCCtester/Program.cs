using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace PCCtester
{
    class Program
    {
        static void Main(string[] args)
        {
            Guid facilityId = new Guid("11111111-1bad-f00d-1111-111111111111");
            string dialstring = "1*111";
            var uri = new Uri("https://pcconsolewebservice.devint.dev-r5ead.net/");

            //Guid facilityId = new Guid("00000000-0000-0000-0000-000000000059");
            //string dialstring = "12*4000";
            //var uri = new Uri("https://pcconsolewebservice.release.dev-r5ead.net/");







            Task.Run( () =>
            // get all the facilities and dialstring and start them up
            IFacilityConfigurationClient facilityClient = new FacilityConfigurationClient();

                var config = await facilityClient.GetPCConsoleServiceConfigAsync();

                if (config?.FacilityGuidsToDialstrings != null)
                {
                    var connectionTasks = config.FacilityGuidsToDialstrings.Select(kv =>
                        kv.Value.Select(v => cas.ConnectToStream(kv.Key, v)));
                    await Task.WhenAll(connectionTasks.SelectMany(o => o).ToList());
                }

                StartClient(facilityId, dialstring, uri.ToString())
            );

            Console.WriteLine("enter to end");
            Console.ReadLine();
        }

        private async static void StartClient(Guid facilityId, string dialstring, string Url)
        {
            for (var count = 1; count <= 20; count++)
            {
                Console.WriteLine($"EXECUTING COUNT:{count}");

                await Task.Delay(TimeSpan.FromSeconds(2));

                try
                {
                    //IPCConsoleServiceClient client = new PCConsoleServiceClient(new Uri(Url), new Uri(identityUrl), TraceLevels.StateChanges);

                    //await client.WireUpAllTouchSignalRConnection(facilityId, dialstring,
                    //    (placed) => Spit(placed),
                    //    (canceled) => Spit(canceled),
                    //    (snapshot) => Spit(snapshot),
                    //    (swingactivated) => Spit(swingactivated),
                    //    (swingdeactivated) => Spit(swingdeactivated),
                    //    servicePlaced => Spit(servicePlaced),
                    //    serviceCanceled => Spit(serviceCanceled),
                    //    (privacyActivated) => Spit(privacyActivated),
                    //    (privacyDeactivated) => Spit(privacyDeactivated),
                    //    (priority) => Spit(priority),
                    //    (answered) => Spit(answered),
                    //    serviceTagUpdated => Spit(serviceTagUpdated),
                    //    tagMappedLocationUpdate => Spit(tagMappedLocationUpdate),
                    //    tagUnmappedLocationUpdate => Spit(tagUnmappedLocationUpdate),
                    //    manualStaffRegisterIn => Spit(manualStaffRegisterIn),
                    //    manualStaffRegisterOut => Spit(manualStaffRegisterOut),
                    //    (s1, s2) => Spit(s1 + s2));


                    HubConnection _streamHubConnection;
                    IHubProxy _hubProxy;

                    _streamHubConnection = new HubConnection(Url.ToString(), new Dictionary<string, string>
                    {
                        {"Dialstring", $"{dialstring}"},
                        {"FacilityGuid", $"{facilityId}"}
                    });

                    //todo this is just a header value... We're going to need some work on the hub to make this work
                    //secure the signalr client in the future
                    //_streamHubConnection.Headers.Add("bearerToken", tokenString);

                    //use https://github.com/SignalR/SignalR/wiki/Tracing-on-the-client-side
                    // specified TraceSources to get trace data
                    //_streamHubConnection.TraceLevel = _traceLevel;
                    //_streamHubConnection.TraceWriter = new CTextWriter();

                    //_streamHubConnection.Error += Connection_Error;
                    //_streamHubConnection.StateChanged += _streamHubConnection_StateChanged;

                    //TODO this needs a code review
                    //_streamHubConnection.Closed += async () => await _streamHubConnection_Closed();


                    _hubProxy = _streamHubConnection.CreateHubProxy("StreamHub");


                    _hubProxy.On<object>("receiveSnapshot", (snapshot) =>
                    {
                        //if (snapshot.FacilityGuid.Equals(facilityId) && snapshot.DialString.Equals(dialstring))
                        //    snapshotHandler(snapshot);
                        Console.WriteLine(snapshot);
                    });

                    try
                    {
                        await _streamHubConnection.Start();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    try
                    {
                        await _hubProxy.Invoke("JoinGroup", facilityId, dialstring);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }

        private static void Spit(object obj)
        {
            //Console.WriteLine($"{obj}");
            Console.Write("...");
        }

    }
}
