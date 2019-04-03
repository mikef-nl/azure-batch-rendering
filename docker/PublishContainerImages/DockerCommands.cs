using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PublishContainerImages
{
    static class DockerCommands
    {
        public static void _runDockerTag(ContainerImageDef imageDef, string localImageId, string tag)
        {
            var _oneMinInMs = 1 * 1000 * 60;

            var commandLine = $"docker tag {localImageId} {imageDef.ContainerImage}:{tag}";

            _runCmdProcess(commandLine, _oneMinInMs);
        }

        public static void _runDockerPush(ContainerImageDef imageDef, string tag)
        {
            var _twentyMinsInMs = 20 * 1000 * 60;

            var commandLine = $"docker push {imageDef.ContainerImage}:{tag}";

            _runCmdProcess(commandLine, _twentyMinsInMs);
        }

        public static string[] _runDockerBuild(string blobSasToken, ContainerImageDef imageDef)
        {
            var _twentyMinsInMs = 20 * 1000 * 60;

            var commandLine = $"build -m 4GB --build-arg INSTALLER_SAS=\"{blobSasToken}\" \"{imageDef.PathToDockerFile}\"";
            return _runCmdProcess(commandLine, _twentyMinsInMs);
        }

        private static string[] _runCmdProcess(string commandLine, int timeoutInMs)
        {
            ProcessStartInfo processStartInfo;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = commandLine,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }
            else
            {
                processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c docker " + commandLine,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
            }
            PublishContainerImages.WriteLog($"Running commandLine: {processStartInfo.FileName} {processStartInfo.Arguments}");
            using (var process = new Process
            {
                StartInfo = processStartInfo
            })
            {
                var output = new List<string>();

                process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        PublishContainerImages.WriteLog(e.Data);
                        output.Add(e.Data);
                    }
                };
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        PublishContainerImages.WriteError(e.Data);
                        output.Add("ERROR: " + e.Data);
                    }
                };
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit(timeoutInMs);
                process.WaitForExit();

                return output.ToArray();
            }
        }
    }
}
