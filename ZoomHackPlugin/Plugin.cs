using Dalamud.Game.Command;
using Dalamud.Plugin;
using System;
using System.Runtime.InteropServices;

namespace ZoomHackPlugin
{
    public class Plugin : IDalamudPlugin
    {
        public string Name => "Zoom Hack";

        private const string commandName = "/zoomhack";

        private DalamudPluginInterface pi;
        private Configuration configuration;
        private AddressResolver resolver;
        private PluginUI ui;

        private int ZoomCurrentOffset = 0x114;
        private int ZoomMaxOffset = 0x11c;
        private int FovCurrentOffset = 0x120;
        private int FovMaxOffset = 0x128;

        public float currentZoomMax
        {
            get
            {
                float[] temp = new float[1];
                Marshal.Copy(resolver.CameraPotisionAddress + ZoomMaxOffset, temp, 0, 1);
                return temp[0];
            }
        }

        public float currentFovMax
        {
            get
            {
                float[] temp = new float[1];
                Marshal.Copy(resolver.CameraPotisionAddress + FovMaxOffset, temp, 0, 1);
                return temp[0];
            }
        }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pi = pluginInterface;

            this.configuration = this.pi.GetPluginConfig() as Configuration ?? new Configuration();
            this.configuration.Initialize(this.pi);

            this.resolver = new AddressResolver();
            this.resolver.Setup(this.pi.TargetModuleScanner);

            this.ui = new PluginUI(this.configuration);
            this.ui.Apply += Apply;

            this.pi.CommandManager.AddHandler(commandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "Shows the config for the zoomhack plugin."
            });

            Apply(this.configuration.zoomMax, this.configuration.fovMax);

            this.pi.UiBuilder.OnBuildUi += DrawUI;
            this.pi.UiBuilder.OnOpenConfigUi += (sender, args) => DrawConfigUI();
        }

        public void Dispose()
        {
            this.ui.Apply -= Apply;

            this.ui.Dispose();

            this.pi.CommandManager.RemoveHandler(commandName);

            this.pi.Dispose();
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            OpenConfigUI();
        }

        private void DrawUI()
        {
            this.ui.Draw();
        }

        private void DrawConfigUI()
        {
            OpenConfigUI();
        }

        private void OpenConfigUI()
        {
            this.ui.zoomMax = currentZoomMax;
            this.ui.fovMax = currentFovMax;
            this.ui.Visible = true;
        }

        public void Apply(float zoomMax, float fovMax)
        {
            var buffer1 = BitConverter.GetBytes(zoomMax);
            Marshal.Copy(buffer1, 0, resolver.CameraPotisionAddress + ZoomMaxOffset, buffer1.Length);
            //Marshal.Copy(buffer1, 0, resolver.CameraPotisionAddress + ZoomCurrentOffset, buffer1.Length);
            var buffer2 = BitConverter.GetBytes(fovMax);
            Marshal.Copy(buffer2, 0, resolver.CameraPotisionAddress + FovMaxOffset, buffer2.Length);
            Marshal.Copy(buffer2, 0, resolver.CameraPotisionAddress + FovCurrentOffset, buffer2.Length);
        }
    }
}
