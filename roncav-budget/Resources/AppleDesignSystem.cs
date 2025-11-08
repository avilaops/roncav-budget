using Microsoft.Maui.Graphics;

namespace roncav_budget.Resources;

/// <summary>
/// Design System inspirado na Apple - Cores, Tipografia e Espaçamentos
/// </summary>
public static class AppleDesignSystem
{
    #region Cores Principais (Apple-like)

    public static class Colors
    {
        // Sistema de cores adaptativo
        public static readonly Color SystemBlue = Color.FromArgb("#007AFF");
        public static readonly Color SystemGreen = Color.FromArgb("#34C759");
        public static readonly Color SystemIndigo = Color.FromArgb("#5856D6");
        public static readonly Color SystemOrange = Color.FromArgb("#FF9500");
        public static readonly Color SystemPink = Color.FromArgb("#FF2D55");
        public static readonly Color SystemPurple = Color.FromArgb("#AF52DE");
        public static readonly Color SystemRed = Color.FromArgb("#FF3B30");
      public static readonly Color SystemTeal = Color.FromArgb("#5AC8FA");
        public static readonly Color SystemYellow = Color.FromArgb("#FFCC00");

        // Tons de cinza (Light Mode)
        public static readonly Color Gray = Color.FromArgb("#8E8E93");
      public static readonly Color Gray2 = Color.FromArgb("#AEAEB2");
        public static readonly Color Gray3 = Color.FromArgb("#C7C7CC");
        public static readonly Color Gray4 = Color.FromArgb("#D1D1D6");
        public static readonly Color Gray5 = Color.FromArgb("#E5E5EA");
        public static readonly Color Gray6 = Color.FromArgb("#F2F2F7");

     // Backgrounds
 public static readonly Color BackgroundPrimary = Color.FromArgb("#FFFFFF");
     public static readonly Color BackgroundSecondary = Color.FromArgb("#F2F2F7");
     public static readonly Color BackgroundTertiary = Color.FromArgb("#FFFFFF");

      // Labels
        public static readonly Color LabelPrimary = Color.FromArgb("#000000");
        public static readonly Color LabelSecondary = Color.FromArgb("#3C3C4399");
     public static readonly Color LabelTertiary = Color.FromArgb("#3C3C434D");

        // Financeiro
   public static readonly Color Income = SystemGreen;
        public static readonly Color Expense = SystemRed;
 public static readonly Color Transfer = SystemBlue;
   public static readonly Color Budget = SystemOrange;
 public static readonly Color Goal = SystemPurple;

     // Card Gradients
        public static readonly Color[] BlueGradient = new[] 
        { 
            Color.FromArgb("#667EEA"), 
     Color.FromArgb("#764BA2") 
        };

        public static readonly Color[] GreenGradient = new[] 
        { 
       Color.FromArgb("#34C759"), 
   Color.FromArgb("#30BE96") 
   };

public static readonly Color[] RedGradient = new[] 
    { 
 Color.FromArgb("#FF3B30"), 
        Color.FromArgb("#FF6B6B") 
  };

public static readonly Color[] PurpleGradient = new[] 
      { 
          Color.FromArgb("#A8EDEA"), 
      Color.FromArgb("#764BA2") 
};
    }

 #endregion

    #region Tipografia (SF Pro-like)

    public static class Typography
    {
        // Tamanhos
  public const double LargeTitle = 34;
        public const double Title1 = 28;
     public const double Title2 = 22;
        public const double Title3 = 20;
        public const double Headline = 17;
     public const double Body = 17;
   public const double Callout = 16;
        public const double Subhead = 15;
        public const double Footnote = 13;
        public const double Caption1 = 12;
        public const double Caption2 = 11;

        // Pesos (usar FontAttributes)
        public const string Regular = "Regular";
public const string Medium = "Medium";
public const string Semibold = "Semibold";
        public const string Bold = "Bold";
    }

    #endregion

    #region Espaçamentos

    public static class Spacing
    {
        public const double XXSmall = 2;
     public const double XSmall = 4;
public const double Small = 8;
        public const double Medium = 12;
        public const double Large = 16;
    public const double XLarge = 20;
        public const double XXLarge = 24;
     public const double Huge = 32;
        public const double Giant = 40;
    }

    #endregion

    #region Corner Radius

    public static class CornerRadius
    {
    public const double Small = 8;
 public const double Medium = 12;
        public const double Large = 16;
        public const double XLarge = 20;
        public const double Pill = 999; // Fully rounded
    }

#endregion

    #region Sombras

    public static class Shadows
    {
     public static readonly Shadow Small = new Shadow
        {
   Brush = new SolidColorBrush(Color.FromRgba("#000000")),
     Offset = new Point(0, 2),
       Radius = 4,
     Opacity = 0.1f
        };

        public static readonly Shadow Medium = new Shadow
        {
   Brush = new SolidColorBrush(Color.FromRgba("#000000")),
       Offset = new Point(0, 4),
            Radius = 8,
  Opacity = 0.15f
     };

        public static readonly Shadow Large = new Shadow
     {
   Brush = new SolidColorBrush(Color.FromRgba("#000000")),
   Offset = new Point(0, 8),
      Radius = 16,
     Opacity = 0.2f
 };
    }

    #endregion

    #region Animações

    public static class Animations
    {
   public const uint Fast = 200;
        public const uint Normal = 300;
  public const uint Slow = 500;

        public static readonly Easing EaseInOut = Easing.CubicInOut;
        public static readonly Easing EaseOut = Easing.CubicOut;
        public static readonly Easing Spring = Easing.SpringOut;
    }

    #endregion

    #region Ícones Iconoir

  public static class Icons
    {
   // Financeiro
        public const string Wallet = "wallet";
        public const string CreditCard = "credit-card";
     public const string Coins = "coins";
public const string DollarCircle = "dollar-circle";
        public const string TrendingUp = "trending-up";
        public const string TrendingDown = "trending-down";
        public const string PiggyBank = "piggy-bank";
        public const string Receipt = "receipt";

     // Navegação
        public const string Home = "home";
        public const string Menu = "menu";
     public const string Settings = "settings";
    public const string Plus = "plus";
        public const string Search = "search";
        public const string Filter = "filter";

     // Categorias
        public const string Shopping = "shopping-bag";
  public const string Food = "food";
        public const string Transport = "car";
        public const string Health = "health";
        public const string Education = "book";
        public const string Entertainment = "gamepad";

     // Status
     public const string Check = "check";
    public const string Warning = "warning-triangle";
        public const string Info = "info-circle";
        public const string Calendar = "calendar";
    }

    #endregion
}
