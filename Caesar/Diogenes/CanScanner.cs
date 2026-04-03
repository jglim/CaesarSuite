using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using SAE.J2534;
using Caesar;

namespace Diogenes
{
    /// <summary>
    /// CAN Bus Scanner - Scans for active ECUs by listening for passive traffic
    /// and actively probing diagnostic CAN ID pairs.
    /// </summary>
    public class CanScanner
    {
        /// <summary>
        /// Runs a full CAN bus scan: passive listen + active diagnostic probe.
        /// Requires a J2534 device to be selected (Connection menu) but no active ECU connection.
        /// </summary>
        public static void RunScan(ECUConnection connection)
        {
            if (connection == null || connection.IsSimulation())
            {
                Console.WriteLine("[Scanner] Cannot scan in simulation mode. Select a real J2534 device first.");
                return;
            }
            if (connection.ConnectionDevice == null)
            {
                Console.WriteLine("[Scanner] No J2534 device available. Please select a device from the Connection menu first.");
                return;
            }

            Console.WriteLine($"\r\n{"=" + new string('=', 59)}");
            Console.WriteLine($"[Scanner] CAN Bus Scanner Starting");
            Console.WriteLine($"[Scanner] Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"[Scanner] Device: {connection.FriendlyName}");
            Console.WriteLine($"{"=" + new string('=', 59)}\r\n");

            // Stop tester present timer during scan
            connection.TesterPresentTimer.Stop();

            // Close any existing channel so we can use the device exclusively
            if (connection.ConnectionChannel != null)
            {
                Console.WriteLine("[Scanner] Closing existing channel for scan...");
                try
                {
                    connection.ConnectionChannel.Dispose();
                }
                catch { }
                connection.ConnectionChannel = null;
            }

            try
            {
                // Phase 1: Passive CAN bus listen
                Dictionary<uint, PassiveResult> passiveResults = PassiveListen(connection.ConnectionDevice, 500000);

                // Phase 2: Active diagnostic scan
                ActiveScan(connection.ConnectionDevice, 500000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Scanner] Fatal error: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"[Scanner] Stack: {ex.StackTrace}");
            }

            Console.WriteLine($"\r\n{"=" + new string('=', 59)}");
            Console.WriteLine("[Scanner] Scan complete.");
            Console.WriteLine("[Scanner] To connect with discovered CAN IDs, use the J2534 Console");
            Console.WriteLine("[Scanner] at the bottom of the window to send raw hex commands.");
            Console.WriteLine($"{"=" + new string('=', 59)}\r\n");

            // Restart tester present timer
            connection.TesterPresentTimer.Start();
        }

        private struct PassiveResult
        {
            public int FrameCount;
            public byte[] LastData;
        }

        /// <summary>
        /// Phase 1: Opens a raw CAN channel and listens for any bus traffic for the specified duration.
        /// This validates the physical layer (wiring, termination, power).
        /// </summary>
        private static Dictionary<uint, PassiveResult> PassiveListen(Device device, int baudrate)
        {
            int listenDurationMs = 5000;
            Console.WriteLine($"[Scanner] === Phase 1: Passive CAN Bus Listen ({listenDurationMs / 1000}s at {baudrate} baud) ===");
            Console.WriteLine("[Scanner] Listening for any CAN traffic on the bus (ignoring Scanmatik internal IDs 0x412)...\r\n");

            var seenIds = new Dictionary<uint, PassiveResult>();
            Channel canChannel = null;

            // CAN IDs known to be Scanmatik internal/echo IDs, not real bus traffic
            HashSet<uint> ignoredIds = new HashSet<uint> { 0x412 };

            try
            {
                canChannel = device.GetChannel(Protocol.CAN, (Baud)baudrate, ConnectFlag.CAN_ID_BOTH);
                Console.WriteLine($"[Scanner] Raw CAN channel opened OK");

                // Pass-all filter: mask=0 means accept everything
                MessageFilter passFilter = new MessageFilter();
                passFilter.FilterType = Filter.PASS_FILTER;
                passFilter.Mask = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                passFilter.Pattern = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                canChannel.StartMsgFilter(passFilter);
                Console.WriteLine("[Scanner] Pass-all filter applied, listening...\r\n");

                Stopwatch sw = Stopwatch.StartNew();

                while (sw.ElapsedMilliseconds < listenDurationMs)
                {
                    try
                    {
                        GetMessageResults result = canChannel.GetMessage();

                        if (result.Result == ResultCode.STATUS_NOERROR)
                        {
                            foreach (Message msg in result.Messages)
                            {
                                // Skip TX confirmations (our own echoes)
                                if (msg.RxStatus.ToString().Contains("TX_MSG_TYPE"))
                                    continue;

                                if (msg.Data.Length >= 4)
                                {
                                    uint canId = (uint)((msg.Data[0] << 24) | (msg.Data[1] << 16) | (msg.Data[2] << 8) | msg.Data[3]);
                                    byte[] data = msg.Data.Skip(4).ToArray();

                                    // Skip known Scanmatik internal IDs
                                    if (ignoredIds.Contains(canId))
                                        continue;

                                    if (seenIds.ContainsKey(canId))
                                    {
                                        var existing = seenIds[canId];
                                        existing.FrameCount++;
                                        existing.LastData = data;
                                        seenIds[canId] = existing;
                                    }
                                    else
                                    {
                                        seenIds[canId] = new PassiveResult { FrameCount = 1, LastData = data };
                                        // Print immediately when we see a new CAN ID
                                        Console.WriteLine($"[Scanner] NEW CAN ID: 0x{canId:X3} @ {sw.ElapsedMilliseconds}ms - Data: [{BitUtility.BytesToHex(data, true)}]");
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Some J2534 implementations throw on empty buffer
                    }
                }

                Console.WriteLine();
                if (seenIds.Count == 0)
                {
                    Console.WriteLine("[Scanner] *** No CAN traffic detected during passive listen (excluding Scanmatik echoes) ***");
                    Console.WriteLine("[Scanner] Possible causes:");
                    Console.WriteLine("[Scanner]   1. Missing CAN bus termination (120 ohm resistor)");
                    Console.WriteLine("[Scanner]   2. CAN-H and CAN-L wires may be swapped");
                    Console.WriteLine("[Scanner]   3. Cluster CAN transceiver not active (check KL15/ignition)");
                    Console.WriteLine("[Scanner]   4. Wrong CAN bus speed (trying 500kbaud, cluster may need 250k or 125k)");
                    Console.WriteLine("[Scanner]   5. Connected to wrong CAN bus pins on the cluster connector");
                }
                else
                {
                    Console.WriteLine($"[Scanner] Passive listen results: {seenIds.Count} unique real CAN ID(s) detected:");
                    Console.WriteLine($"[Scanner] {"CAN ID",-12} {"Frames",-10} {"Last Data"}");
                    Console.WriteLine($"[Scanner] {new string('-', 60)}");
                    foreach (var kvp in seenIds.OrderBy(x => x.Key))
                    {
                        string dataStr = kvp.Value.LastData.Length > 0 ? BitUtility.BytesToHex(kvp.Value.LastData, true) : "(empty)";
                        Console.WriteLine($"[Scanner]   0x{kvp.Key:X3}       {kvp.Value.FrameCount,-10} {dataStr}");
                    }
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Scanner] Phase 1 error: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine("[Scanner] Note: some J2534 devices may not support raw CAN mode.");
            }
            finally
            {
                try { canChannel?.Dispose(); } catch { }
            }

            return seenIds;
        }

        /// <summary>
        /// Phase 2: Uses ISO15765 channels to actively probe diagnostic CAN ID pairs.
        /// This avoids triggering the Scanmatik's internal FC auto-reply which pollutes raw CAN scans.
        /// Also focused on 0x408 which appears to be a real cluster periodic broadcast.
        /// </summary>
        private static void ActiveScan(Device device, int baudrate)
        {
            Console.WriteLine("[Scanner] === Phase 2: Active Diagnostic ISO15765 Scan ===");
            Console.WriteLine("[Scanner] NOTE: Previous raw CAN scan showed 0x408 broadcasting real cluster data.");
            Console.WriteLine("[Scanner] NOTE: 0x412 was the Scanmatik internal FC echo - NOT a real ECU response.");
            Console.WriteLine("[Scanner] This phase uses ISO15765 channels to find the actual diagnostic TX/RX pair.\r\n");

            var respondingIds = new List<Tuple<int, int, byte[]>>(); // tx, rx, response data

            // Candidate pairs derived from the cluster's broadcast ID 0x408:
            // Common patterns: ECU tx = addr, ECU rx = addr+8 OR addr-8 OR nearby
            // 0x408 broadcasting -> likely its TX = 0x408, diagnostic request sent TO it might be on 0x400, 0x410, 0x408, etc.
            // Also try standard Mercedes patterns starting from 0x408 cluster address.
            var candidatePairs = new List<(int tx, int rx, string note)>
            {
                // Based on cluster broadcasting 0x408: 
                // In Mercedes, functional/diag TX is typically ECU_ADDR, tester->ECU is ECU_ADDR±8
                (0x400, 0x408, "Tester->0x400, Cluster->0x408 (standard +8 pattern)"),
                (0x408, 0x400, "Tester->0x408, Cluster->0x400 (reverse)"),
                (0x410, 0x408, "Tester->0x410, Cluster->0x408"),
                (0x408, 0x410, "Tester->0x408, Cluster->0x410"),
                (0x408, 0x408, "Same ID (loop test)"),
                // CBF values from ki211.cbf:
                (0x5B4, 0x4F4, "CBF profile values TX=0x5B4, RX=0x4F4"),
                (0x4F4, 0x5B4, "CBF reversed: TX=0x4F4, RX=0x5B4"),
                // Standard UDS range near 0x700
                (0x700, 0x708, "Standard 0x700->0x708"),
                (0x710, 0x718, "Standard 0x710->0x718"),
                (0x720, 0x728, "Standard 0x720->0x728"),
                (0x730, 0x738, "Standard 0x730->0x738"),
                (0x740, 0x748, "Standard 0x740->0x748"),
                (0x750, 0x758, "Standard 0x750->0x758"),
                (0x760, 0x768, "Standard 0x760->0x768"),
                (0x770, 0x778, "Standard 0x770->0x778"),
                // OBD-II broadcast
                (0x7DF, 0x7E8, "OBD-II functional"),
            };

            // Diagnostic payloads to try (actual UDS/KW2C3PE service bytes, NO raw CAN PCI wrapper)
            // These are the ISO-TP payloads -- the J2534 ISO15765 stack will handle framing
            byte[][] diagnosticPayloads = new byte[][]
            {
                new byte[] { 0x10, 0x03 },  // DiagSessionControl Extended (KW2C3PE and UDS)
                new byte[] { 0x10, 0x92 },  // DiagSessionControl KW2C3PE mode
                new byte[] { 0x10, 0x01 },  // DiagSessionControl Default
                new byte[] { 0x3E, 0x00 },  // TesterPresent suppress response
                new byte[] { 0x3E, 0x01 },  // TesterPresent KW2C3PE
                new byte[] { 0x1A, 0x86 },  // ReadECUIdentification (KW2C3PE)
            };

            string[] payloadDescriptions = new string[]
            {
                "DiagSession Extended (10 03)",
                "DiagSession KW2C3PE (10 92)",
                "DiagSession Default (10 01)",
                "TesterPresent UDS (3E 00)",
                "TesterPresent KW2C3PE (3E 01)",
                "ReadECUID KW2C3PE (1A 86)",
            };

            Console.WriteLine($"[Scanner] Testing {candidatePairs.Count} candidate TX/RX pairs with {diagnosticPayloads.Length} probes each\r\n");

            foreach (var pair in candidatePairs)
            {
                Console.WriteLine($"[Scanner] Testing: {pair.note}");
                Console.WriteLine($"[Scanner]   TX=0x{pair.tx:X3} RX=0x{pair.rx:X3}");

                Channel isoChannel = null;
                try
                {
                    isoChannel = device.GetChannel(Protocol.ISO15765, (Baud)baudrate, ConnectFlag.CAN_ID_BOTH);
                    isoChannel.DefaultTxFlag = TxFlag.ISO15765_FRAME_PAD;

                    // Configure flow control filter for this pair
                    byte[] txBytes = BitConverter.GetBytes(pair.tx); Array.Reverse(txBytes);
                    byte[] rxBytes = BitConverter.GetBytes(pair.rx); Array.Reverse(rxBytes);

                    MessageFilter filter = new MessageFilter();
                    filter.FilterType = Filter.FLOW_CONTROL_FILTER;
                    filter.Mask = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
                    filter.Pattern = rxBytes;
                    filter.FlowControl = txBytes;
                    filter.TxFlags = TxFlag.ISO15765_FRAME_PAD;
                    isoChannel.ClearMsgFilters();
                    isoChannel.StartMsgFilter(filter);
                    isoChannel.ClearRxBuffer();
                    isoChannel.ClearTxBuffer();

                    foreach (var payload in diagnosticPayloads.Zip(payloadDescriptions, (p, d) => (p, d)))
                    {
                        // Build ISO15765 packet: TX CAN ID bytes + payload
                        List<byte> packet = new List<byte>(txBytes);
                        packet.AddRange(payload.p);

                        try { isoChannel.ClearRxBuffer(); } catch { }

                        try
                        {
                            isoChannel.SendMessage(packet);
                        }
                        catch (Exception sendEx)
                        {
                            Console.WriteLine($"[Scanner]   Send failed for {payload.d}: {sendEx.Message}");
                            continue;
                        }

                        // Poll for response
                        Stopwatch responseTimer = Stopwatch.StartNew();
                        bool gotResponse = false;
                        while (responseTimer.ElapsedMilliseconds < 500)
                        {
                            try
                            {
                                GetMessageResults readResult = isoChannel.GetMessage();
                                if (readResult.Result == ResultCode.STATUS_NOERROR)
                                {
                                    foreach (Message msg in readResult.Messages)
                                    {
                                        if (msg.RxStatus.ToString().Contains("TX_MSG_TYPE"))
                                            continue;

                                        if (msg.Data.Length >= 5)
                                        {
                                            uint rxCanId = (uint)((msg.Data[0] << 24) | (msg.Data[1] << 16) | (msg.Data[2] << 8) | msg.Data[3]);
                                            byte[] rxData = msg.Data.Skip(4).ToArray();

                                            Console.WriteLine($"[Scanner]   *** RESPONSE! Probe={payload.d}");
                                            Console.WriteLine($"[Scanner]   RX CAN ID: 0x{rxCanId:X3}  Data: [{BitUtility.BytesToHex(rxData, true)}]");

                                            if (rxData.Length >= 2)
                                            {
                                                byte svcByte = rxData[0];
                                                if (svcByte == 0x7F && rxData.Length >= 3)
                                                    Console.WriteLine($"[Scanner]   Interpretation: Negative Response (Service 0x{rxData[1]:X2}, NRC 0x{rxData[2]:X2})");
                                                else if (svcByte == 0x50)
                                                    Console.WriteLine($"[Scanner]   Interpretation: Positive DiagSessionControl!");
                                                else if (svcByte == 0x7E)
                                                    Console.WriteLine($"[Scanner]   Interpretation: Positive TesterPresent!");
                                                else if (svcByte == 0x5A)
                                                    Console.WriteLine($"[Scanner]   Interpretation: Positive ReadECUIdentification!");
                                                else
                                                    Console.WriteLine($"[Scanner]   Interpretation: Service response 0x{svcByte:X2}");
                                            }

                                            respondingIds.Add(new Tuple<int, int, byte[]>(pair.tx, pair.rx, rxData));
                                            gotResponse = true;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }

                        if (!gotResponse)
                        {
                            Console.WriteLine($"[Scanner]   No response to {payload.d} in 500ms");
                        }
                        Thread.Sleep(50);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Scanner]   Channel error: {ex.Message}");
                }
                finally
                {
                    try { isoChannel?.Dispose(); } catch { }
                }
                Console.WriteLine();
            }

            // Summary
            Console.WriteLine($"[Scanner] === Scan Summary ===");
            Console.WriteLine($"[Scanner] Responding pairs: {respondingIds.Count}");

            if (respondingIds.Count > 0)
            {
                Console.WriteLine($"\r\n[Scanner] *** Confirmed diagnostic endpoints:");
                var uniquePairs = respondingIds
                    .GroupBy(x => new { TX = x.Item1, RX = x.Item2 })
                    .Select(g => g.First());

                foreach (var p in uniquePairs)
                    Console.WriteLine($"[Scanner]   TX=0x{p.Item1:X3}  RX=0x{p.Item2:X3}  Data=[{BitUtility.BytesToHex(p.Item3, true)}]");
            }
            else
            {
                Console.WriteLine($"\r\n[Scanner] No ISO15765 responses found.");
                Console.WriteLine("[Scanner] However, 0x408 was seen broadcasting in Phase 1.");
                Console.WriteLine("[Scanner] This means the cluster CAN transceiver IS active.");
                Console.WriteLine("[Scanner] Most likely: missing 120 ohm CAN bus termination resistor.");
                Console.WriteLine("[Scanner]   -> The cluster broadcasts periodic frames regardless,");
                Console.WriteLine("[Scanner]      but without proper termination, query-response frames");
                Console.WriteLine("[Scanner]      get corrupted or the cluster CAN controller drops them.");
                Console.WriteLine("[Scanner] Try: add 120 ohm resistor between CAN-H and CAN-L at J2534 side.");
            }
        }
    }
}
