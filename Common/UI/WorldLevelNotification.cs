using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using System;

namespace Progression.Common.UI
{
    /// <summary>
    /// Visual notification system for world level changes
    /// Displays an achievement-style banner at the top of the screen
    /// </summary>
    public class WorldLevelNotificationUI : UIState
    {
        private static WorldLevelNotificationUI _instance;
        public static WorldLevelNotificationUI Instance => _instance ??= new WorldLevelNotificationUI();

        // Notification data
        private string titleText = "";
        private string levelText = "";
        private string subtitleText = "";
        private Color titleColor = Color.Gold;
        
        // Animation state
        private float animationProgress = 0f;
        private const float SLIDE_IN_DURATION = 30f;  // frames
        private const float DISPLAY_DURATION = 180f;  // frames (3 seconds)
        private const float SLIDE_OUT_DURATION = 30f; // frames
        private const float TOTAL_DURATION = SLIDE_IN_DURATION + DISPLAY_DURATION + SLIDE_OUT_DURATION;
        
        private bool isActive = false;
        private float timer = 0f;

        // Visual settings
        private const float WIDTH = 500f;
        private const float HEIGHT = 120f;
        private const float PADDING = 20f;

        /// <summary>
        /// Shows a world level notification
        /// </summary>
        public static void Show(int newLevel, string reason = "")
        {
            var instance = Instance;
            instance.titleText = "WORLD LEVEL UP!";
            instance.levelText = $"Level {newLevel}";
            instance.subtitleText = string.IsNullOrEmpty(reason) 
                ? "Enemies grow stronger..." 
                : reason;
            instance.titleColor = Color.Gold;
            instance.isActive = true;
            instance.timer = 0f;
            instance.animationProgress = 0f;

            // Play epic sound
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.Item4);
        }

        public override void Update(GameTime gameTime)
        {
            if (!isActive)
                return;

            timer++;
            animationProgress = timer / TOTAL_DURATION;

            if (timer >= TOTAL_DURATION)
            {
                isActive = false;
                timer = 0f;
                animationProgress = 0f;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!isActive)
                return;

            // Calculate animation offset
            float yOffset = GetYOffset();
            
            // Screen center top
            float screenCenterX = Main.screenWidth / 2f;
            float baseY = 80f + yOffset;

            // Draw semi-transparent background
            DrawBackground(spriteBatch, screenCenterX, baseY);

            // Draw border and glow
            DrawBorder(spriteBatch, screenCenterX, baseY);

            // Draw text content
            DrawText(spriteBatch, screenCenterX, baseY);
        }

        private float GetYOffset()
        {
            // Slide in from top
            if (timer < SLIDE_IN_DURATION)
            {
                float progress = timer / SLIDE_IN_DURATION;
                return MathHelper.Lerp(-HEIGHT - 50f, 0f, EaseOutBack(progress));
            }
            // Stay in place
            else if (timer < SLIDE_IN_DURATION + DISPLAY_DURATION)
            {
                return 0f;
            }
            // Slide out to top
            else
            {
                float progress = (timer - SLIDE_IN_DURATION - DISPLAY_DURATION) / SLIDE_OUT_DURATION;
                return MathHelper.Lerp(0f, -HEIGHT - 50f, EaseInBack(progress));
            }
        }

        private void DrawBackground(SpriteBatch spriteBatch, float centerX, float y)
        {
            // Main background
            Rectangle bgRect = new Rectangle(
                (int)(centerX - WIDTH / 2f),
                (int)y,
                (int)WIDTH,
                (int)HEIGHT
            );

            // Dark background with transparency
            Color bgColor = new Color(10, 10, 30, 220);
            DrawRectangle(spriteBatch, bgRect, bgColor);

            // Inner glow
            Rectangle innerGlow = new Rectangle(
                bgRect.X + 2,
                bgRect.Y + 2,
                bgRect.Width - 4,
                bgRect.Height - 4
            );
            Color glowColor = new Color(titleColor.R, titleColor.G, titleColor.B, 50);
            DrawRectangle(spriteBatch, innerGlow, glowColor);
        }

        private void DrawBorder(SpriteBatch spriteBatch, float centerX, float y)
        {
            Rectangle bgRect = new Rectangle(
                (int)(centerX - WIDTH / 2f),
                (int)y,
                (int)WIDTH,
                (int)HEIGHT
            );

            // Animated glow border
            float pulseAmount = (float)Math.Sin(timer * 0.1f) * 0.3f + 0.7f;
            Color borderColor = titleColor * pulseAmount;

            // Top border
            DrawRectangle(spriteBatch, new Rectangle(bgRect.X, bgRect.Y, bgRect.Width, 3), borderColor);
            // Bottom border
            DrawRectangle(spriteBatch, new Rectangle(bgRect.X, bgRect.Bottom - 3, bgRect.Width, 3), borderColor);
            // Left border
            DrawRectangle(spriteBatch, new Rectangle(bgRect.X, bgRect.Y, 3, bgRect.Height), borderColor);
            // Right border
            DrawRectangle(spriteBatch, new Rectangle(bgRect.Right - 3, bgRect.Y, 3, bgRect.Height), borderColor);
        }

        private void DrawText(SpriteBatch spriteBatch, float centerX, float y)
        {
            // Title "WORLD LEVEL UP!"
            DrawCenteredText(spriteBatch, titleText, centerX, y + PADDING, 1.2f, titleColor);

            // Level number (large)
            DrawCenteredText(spriteBatch, levelText, centerX, y + PADDING + 35f, 1.5f, Color.White);

            // Subtitle
            DrawCenteredText(spriteBatch, subtitleText, centerX, y + PADDING + 75f, 0.8f, Color.OrangeRed);
        }

        private void DrawCenteredText(SpriteBatch spriteBatch, string text, float x, float y, float scale, Color color)
        {
            var font = FontAssets.MouseText.Value;
            Vector2 textSize = font.MeasureString(text) * scale;
            Vector2 position = new Vector2(x - textSize.X / 2f, y);

            // Draw shadow
            Utils.DrawBorderString(spriteBatch, text, position, color * 0.8f, scale);
        }

        private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            // Use a white pixel texture to draw rectangles
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, rect, color);
        }

        // Easing functions for smooth animation
        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * (float)Math.Pow(t - 1f, 3f) + c1 * (float)Math.Pow(t - 1f, 2f);
        }

        private float EaseInBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }
    }

    /// <summary>
    /// System to manage the notification UI layer
    /// </summary>
    public class WorldLevelNotificationSystem : ModSystem
    {
        private UserInterface _notificationInterface;
        private WorldLevelNotificationUI _notificationUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                _notificationInterface = new UserInterface();
                _notificationUI = WorldLevelNotificationUI.Instance;
                _notificationInterface.SetState(_notificationUI);
            }
        }

        public override void Unload()
        {
            _notificationInterface = null;
            _notificationUI = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (_notificationInterface?.CurrentState != null)
            {
                _notificationInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
                    "SignatureEquipmentDeluxe: World Level Notification",
                    delegate
                    {
                        if (_notificationInterface?.CurrentState != null)
                        {
                            _notificationInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
