using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureEdit
{
    public partial class VersionInformationDialog : Form
    {
        List<VersionInfo> versions = new List<VersionInfo>()
        {
            new VersionInfo("v1.20.31 (Current Version)", new List<string>()
            {
                "Bug fix"
            }),
            new VersionInfo("v1.20.30", new List<string>()
            {
                "Fixed a bug that caused TextureEdit to not start if you had no default image editor set in explorer"
            }),
            new VersionInfo("v1.20.29", new List<string>()
            {
                "lol nothing :)"
            }),
            new VersionInfo("v1.20.28", new List<string>()
            {
                "TextureEdit now supports all external editors that register with the Windows shell (almost all editors)",
                "Closing will result in are you sure dialog"
            }),
            new VersionInfo("v1.20.27", new List<string>()
            {
                "Fixed a small shadow bug"
            }),
            new VersionInfo("v1.20.26", new List<string>()
            {
                "Minimum resolution is now 1x1"
            }),
            new VersionInfo("v1.20.25", new List<string>()
            {
                "Fixed a shadow bug"
            }),
            new VersionInfo("v1.20.24", new List<string>()
            {
                "Automatic TgaLib.dll download"
            }),
            new VersionInfo("v1.20.23", new List<string>()
            {
                "Drop shadows even nicer :) also for some reason making the drop shadows better improved performance so yay!!!!"
            }),
            new VersionInfo("v1.20.22", new List<string>()
            {
                "Drop shadows in the UI look a lil nicer"
            }),
            new VersionInfo("v1.20.21", new List<string>()
            {
                "You can now \"switch out\" tiles in the tile view with click and right click, useful for developing things like stone and ores"
            }),
            new VersionInfo("v1.20.20", new List<string>()
            {
                "Created tile view feature (dm me if you wanna know what that means)"
            }),
            new VersionInfo("v1.20.19", new List<string>()
            {
                "Fixed a bug where smart selection filling wouldn't update until you moused over the workspace"
            }),
            new VersionInfo("v1.20.18", new List<string>()
            {
                "Holding shift with Ctrl+S ignores opacity and always saves as png, holding alt doesn't save mers and normals, and any combination of these"
            }),
            new VersionInfo("v1.20.16", new List<string>()
            {
                "Fixed a small interpolation bug."
            }),
            new VersionInfo("v1.20.15", new List<string>()
            {
                "Fixed that bug where the window would stutter when it went off the screen (uhhh it was a weird fix)",
                "Added Ctrl click to magic wand to select all of that color in the whole image",
                "Fixed a bug where downscaling would sometimes cause the workspace to become very blurry"
            }),
            new VersionInfo("v1.20.14", new List<string>()
            {
                "Filling of smart selections is now faster",
                "Added Ctrl+A"
            }),
            new VersionInfo("v1.20.13", new List<string>()
            {
                "Smart selections/wand selections now render way faster which was previously causing wand to be unusable on 1024.",
                "Wand tool now supports channel modes."
            }),
            new VersionInfo("v1.20.12", new List<string>()
            {
                "Effects on smart selections are now wayyy faster"
            }),
            new VersionInfo("v1.20.11", new List<string>()
            {
                "Wand tool is now wayyy faster and available up to 1024: Fixed a bug and added W shortcut edition"
            }),
            new VersionInfo("v1.20.10", new List<string>()
            {
                "Wand tool is now wayyy faster and available up to 1024"
            }),
            new VersionInfo("v1.20.9", new List<string>()
            {
                "Added magic wand tool available up to 128"
            }),
            new VersionInfo("v1.20.8", new List<string>()
            {
                "Made a small improvement to Edit in Paint.net",
                "Added rescaling up to 1k",
                "Added solid background option"
            }),
            new VersionInfo("v1.20.7", new List<string>()
            {
                "Fixed a bug with Edit in Paint.net breaking while in channel modes"
            }),
            new VersionInfo("v1.20.6", new List<string>()
            {
                "fixed a bug. how did i fix it? ion really kno tbh"
            }),
            new VersionInfo("v1.20.5", new List<string>()
            {
                "huuuuge optimizations for high resolutions. Try opening a 1024 or sum :)"
            }),
            new VersionInfo("v1.20.4", new List<string>()
            {
                "Fixed a bug where the workspace wouldn't refresh immediately when moving mouse quickly in channel mode",
                "Optimizations"
            }),
            new VersionInfo("v1.20.3", new List<string>()
            {
                "Fixed a bug where the checker-board transparency background would get stretched on non-square textures"
            }),
            new VersionInfo("v1.20.2", new List<string>()
            {
                "Bug fixes"
            }),
            new VersionInfo("v1.20.1", new List<string>()
            {
                "ok now undo & redo is ACTUALLY full. Unless i forgot something again... nahh"
            }),
            new VersionInfo("v1.20", new List<string>()
            {
                "Added full undo & redo!",
                "Added text boxes for sliders in effects",
                "Updated help information about undo"
            }),
            new VersionInfo("v1.19.21", new List<string>()
            {
                "Pen improvements and channel mode optimizations"
            }),
            new VersionInfo("v1.19.20", new List<string>()
            {
                "Fixed a bug that made the effects preview lag behind"
            }),
            new VersionInfo("v1.19.9", new List<string>()
            {
                "ITS EVEN FASTER JEEBUS"
            }),
            new VersionInfo("v1.19.8", new List<string>()
            {
                "oh my god it's so much faster i feel so free when i select things without a stutter"
            }),
            new VersionInfo("v1.19.7", new List<string>()
            {
                "Optimizations, performance could now be considered suitable for 512x512 textures"
            }),
            new VersionInfo("v1.19.6", new List<string>()
            {
                "Big improvements to the pen tool",
                "Minor optimizations"
            }),
            new VersionInfo("v1.19.5", new List<string>()
            {
                "Fixed a bug that I made while fixing a different bug",
            }),
            new VersionInfo("v1.19.4", new List<string>()
            {
                "R to reset zoom.",
                "Small UI improvements",
            }),
            new VersionInfo("v1.19.3", new List<string>()
            {
                "Fixed outdated information about the zoom tool in the help dialog.",
                "Added unreleased mode to get TextureEdit to shutup about updating to what is actually an older version.",
                "Fixed the most annoying bug of all time"
            }),
            new VersionInfo("v1.19.2", new List<string>()
            {
                "Decreased the minimum size of the window so that you can edit with two windows in split screen"
            }),
            new VersionInfo("v1.19.1", new List<string>()
            {
                "Fixed the worst bug to ever exist in the history of TextureEdit"
            }),
            new VersionInfo("v1.19", new List<string>()
            {
                "Resizable window and workspace!!!",
                "Zoom tool works like select tool!!!!!!"
            }),
            new VersionInfo("v1.18.10", new List<string>()
            {
                "Bug fixes"
            }),
            new VersionInfo("v1.18.9", new List<string>()
            {
                "Quick & dirty support for irregularly sized textures"
            }),
            new VersionInfo("v1.18.8", new List<string>()
            {
                "Added greyscale mode"
            }),
            new VersionInfo("v1.18.7", new List<string>()
            {
                "Grid & Opacity settings",
                "Bug fixes with zoom"
            }),
            new VersionInfo("v1.18.6", new List<string>()
            {
                "Auto updates",
                "Whole lotta bug fixes"
            }),
            new VersionInfo("v1.18.3", new List<string>()
            {
                "More bug fixes for zoom tool"
            }),
            new VersionInfo("v1.18.2", new List<string>()
            {
                "Help dialog",
                "Bug fixes for zoom tool",
                "Bug fixes with opening and saving",
                "2nd release version!!",
            }),
            new VersionInfo("v1.18.1", new List<string>()
            {
                "Much needed zoom tool",
            }),
            new VersionInfo("v1.18", new List<string>()
            {
                "Brush sizzzzzeeee",
            }),
            new VersionInfo("v1.17.4", new List<string>()
            {
                "Crappy larger resolution support",
            }),
            new VersionInfo("v1.17.3", new List<string>()
            {
                "Smart select",
                "First release version!!"
            }),
            new VersionInfo("v1.17.2", new List<string>()
            {
                "Crappy undo for pen"
            }),
            new VersionInfo("v1.17.1", new List<string>()
            {
                "Added current file in window title",
                "Added tooltips for buttons to specify their keyboard shortcut"
            }),
            new VersionInfo("v1.17", new List<string>()
            {
                "Added color shifting",
                "Added apply to all frames on effects"
            }),
            new VersionInfo("v1.16.1", new List<string>()
            {
                "Fixed TGA Bugs... Almost"
            }),
            new VersionInfo("v1.16", new List<string>()
            {
                "Added TGA support/In-game opacity support!!!!!! YAY"
            }),
            new VersionInfo("v1.15", new List<string>()
            {
                "ACTUALLY added back alpha capabilities... Haha... Ha."
            }),
            new VersionInfo("v1.14", new List<string>()
            {
                "Added version information",
                "Added alpha back (...see next version)",
                "Added flipbook animation test pause/play",
                "Fixed height map bug (FINALLY!!!)"
            }),
            new VersionInfo("v1.13", new List<string>()
            {
                "Removed alpha capabilities for testing purposes"
            }),
            new VersionInfo("End of our version logging", new List<string>()
            {
                "We weren't logging version changes until v1.13+"
            })
        };
        public VersionInformationDialog()
        {
            InitializeComponent();
            versionsBox.Items.Clear();
            foreach (VersionInfo info in versions)
            {
                versionsBox.Items.Add(info.VersionName);
            }
            versionsBox.SelectedIndex = 0;
        }

        private void versionsBox_SelectedValueChanged(object sender, EventArgs e)
        {
            changesBox.Items.Clear();
            VersionInfo info = versions[versionsBox.SelectedIndex];
            foreach (string change in info.VersionChanges)
            {
                changesBox.Items.Add(change);
            }
        }
        private class VersionInfo
        {
            public string VersionName;
            public List<string> VersionChanges;
            public VersionInfo(string Name, List<string> Changes)
            {
                VersionName = Name;
                VersionChanges = Changes;
            }
        }
    }
}
