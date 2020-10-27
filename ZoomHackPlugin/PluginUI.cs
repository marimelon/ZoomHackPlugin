using ImGuiNET;
using System;
using System.Numerics;

namespace ZoomHackPlugin
{
    class PluginUI : IDisposable
    {
        private Configuration configuration;

        private float zoomMaxDefault = 20f;
        private float fovMaxDefault = 0.78f;

        public float zoomMax;
        public float fovMax;

        // this extra bool exists for ImGui, since you can't ref a property
        private bool visible = false;
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public delegate void ApplyHandler(float zoomMax, float fieldOfView);
        public event ApplyHandler Apply;

        // passing in the image here just for simplicity
        public PluginUI(Configuration configuration)
        {
            this.configuration = configuration;
            zoomMax = configuration.zoomMax;
            fovMax = configuration.fovMax;
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawMainWindow();
        }

        public void DrawMainWindow()
        {
            if (!Visible)
            {
                return;
            }

            ImGui.SetNextWindowSize(new Vector2(375, 180), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSizeConstraints(new Vector2(375, 180), new Vector2(float.MaxValue, float.MaxValue));
            if (ImGui.Begin("Zoom Hack", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                ImGui.Text("Zoom Max:");
                ImGui.InputFloat("##zoomMax", ref zoomMax, 1f);
                ImGui.SameLine();
                if (ImGui.Button("Default##zoomMax"))
                {
                    zoomMax = zoomMaxDefault;
                }

                ImGui.Text("Field of View:");
                ImGui.InputFloat("##fovMax", ref fovMax, 0.01f);
                ImGui.SameLine();
                if (ImGui.Button("Default##fovMax"))
                {
                    fovMax = fovMaxDefault;
                }

                if (ImGui.Button("Apply"))
                {
                    Apply(zoomMax, fovMax);
                }
                if (ImGui.Button("Save and Close Config"))
                {
                    Apply(zoomMax, fovMax);
                    SaveConfig();
                    Visible = false;
                }
            }
            ImGui.End();
        }

        private void SaveConfig()
        {
            configuration.zoomMax = this.zoomMax;
            configuration.fovMax = this.fovMax;
            configuration.Save();
        }
    }
}
