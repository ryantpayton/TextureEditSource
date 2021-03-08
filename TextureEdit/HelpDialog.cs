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
    public partial class HelpDialog : Form
    {
        private List<HelpTopic> helpTopics = new List<HelpTopic>()
        {
            new HelpTopic("Pen/Brush Tool", @"The pen tool is used to draw on your texture. Use the sliders in the bottom left to control the color or in greyscale channels the value you are painting (how emissive, rough, etc). Use the scroll wheel to move your brush size up and down, or two finger swipes on a track pad.", "P key"),
            new HelpTopic("Select Tool", @"The select tool allows you to edit a region of your texture. Holding shift will allows allow you to select like a pen tool so you can select in non-rectangular shapes. Changing the sliders in the bottom left will edit the color of your entire selection, the select tool is also used for color shifting, greyscale adjustments, and copy/paste.", "O key, hold shift for smart select"),
            new HelpTopic("Picker Tool", @"The picker tool is used to grab a color or value from your texture that you've already painted. When moving your cursor around with the picker tool, you'll be able to see a preview of what you're selecting in the bottom left. Click to select this color.", "I key"),
            new HelpTopic("Zoom Tool", @"The zoom tool is used to work with a smaller area of your texture, it's very useful for higher resolution textures. When in the zoom tool, select an area you'd like to zoom in on. When in other tools, this region will be blown up. Go back into the zoom tool and change your zoom region at any time, press R or select the whole area to zoom all the way out.", "Z key, R to reset zoom"),
            new HelpTopic("Channels", @"The buttons on the middle left is how you change what you're editing. The metallic, emissive, roughness and height map modes are all black and white. The brighter the color, the higher the value (more metallic, etc). The sliders at the bottom left will change to match the channel, for example in height map, it will show only one slider between lowered and raised", "Ctrl+T, Ctrl+M, Ctrl+E, Ctrl+R, Ctrl+H"),
            new HelpTopic("Opening && Saving", @"Using Ctrl+S, you can save a texture. Give it a name, and TextureEdit will automatically save a texture of that name, as well as the _mer and _normal files. TextureEdit will detect if the texture has opacity and if so will save as a tga. When opening with Ctrl+O, open the base texture and TextureEdit will detect if there is an _mer and _normal to open. If there isn't, the current mer and normal will stay loaded.", "Ctrl+S, Ctrl+O"),
            new HelpTopic("Scaling && Resolutions", @"The buttons at the top of the workspace allow you to change the resolution of your texture between 16x16 through 128x128. Notice that scaling down your texture will lose detail. TextureEdit also (technically) has the ability to open up to 512x512 textures, but it has poor support past 128 and there may be bugs.", "No shortcuts"),
            new HelpTopic("Undo & Redo", @"Press Ctrl+Z to undo the last thing you just did, and ctrl+Y to redo! The undo history goes back infinitely", "Ctrl+Z, Ctrl+Y"),
            new HelpTopic("Color Shifting", @"Only in the texture channel, the color shift button allows you to shift the colors in your selection. The RGBA sliders allow you to offset the color of each individual pixel within your selection. The use HSL checkbox changes the sliders to use HSLA instead of RGBA. The Apply to All Frames button will cause this change to be recreated across all flipbook frames (only in flipbook mode, see Flipbook textures)", "No shortcuts"),
            new HelpTopic("Greyscale Adjustments", @"Similar to color shifting, but only available in greyscale channels. This allows you to change the contrast and offset/brightness of individual pixels. The contrast is useful for things such as making a height map's bumps further apart, and the offset could be used to bring them all up. There is also an Apply Offset First (before applying contrast) checkbox, an Invert checkbox, and Apply to all frames like in Color Shifting.", "No shortcuts"),
            new HelpTopic("Copy/paste", @"Ctrl+C and Ctrl+V allow you to copy and paste your selection in and out of the program as well as in between channels. Copying from the texture channel to a greyscale channel will automatically turn it greyscale, this is super useful for a quick way to create height maps.", "Ctrl+C, Ctrl+V"),
            new HelpTopic("Flipbook Textures", @"Some minecraft textures are animated, these are called flipbook textures. TextureEdit will detect if you open an animated texture, arrows at the bottom of the workspace will appear allowing you to change which frame you're editing. A play/pause button will also allow you to preview the animation.", "No shortcuts"),
            new HelpTopic("More help, updates, and more in r/TextureEdit!", @"reddit.com/r/TextureEdit is the home to TextureEdit's updates and a place where you can ask questions, make suggestions and more. If you didn't get help here, ask a question in r/TextureEdit!", "No shortcuts")
        };
        public HelpDialog()
        {
            InitializeComponent();
            topic.SelectedIndex = 0;
        }

        private void topic_SelectedIndexChanged(object sender, EventArgs e)
        {
            title.Text = helpTopics[topic.SelectedIndex].Title;
            description.Text = helpTopics[topic.SelectedIndex].Description;
            shortcuts.Text = helpTopics[topic.SelectedIndex].Shortcuts;
        }

        private class HelpTopic
        {
            public string Title;
            public string Description;
            public string Shortcuts;
            public HelpTopic(string Title, string Description, string Shortcuts)
            {
                this.Title = Title;
                this.Description = Description;
                this.Shortcuts = Shortcuts;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
