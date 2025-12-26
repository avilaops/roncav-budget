namespace Orcamento.Behaviors;

/// <summary>
/// Behavior para animação de entrada (Fade + Slide)
/// </summary>
public class FadeInBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(uint), typeof(FadeInBehavior), 300u);

    public static readonly BindableProperty DelayProperty =
        BindableProperty.Create(nameof(Delay), typeof(uint), typeof(FadeInBehavior), 0u);

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public uint Delay
    {
        get => (uint)GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Opacity = 0;
        bindable.TranslationY = 20;

        _ = AnimateAsync(bindable);
    }

    private async Task AnimateAsync(VisualElement element)
    {
        if (Delay > 0)
            await Task.Delay((int)Delay);

        await Task.WhenAll(
            element.FadeTo(1, Duration),
            element.TranslateTo(0, 0, Duration, Easing.CubicOut)
        );
    }
}

/// <summary>
/// Behavior para animação de Scale ao aparecer
/// </summary>
public class ScaleInBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty DurationProperty =
        BindableProperty.Create(nameof(Duration), typeof(uint), typeof(ScaleInBehavior), 300u);

    public uint Duration
    {
        get => (uint)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Opacity = 0;
        bindable.Scale = 0.8;

        _ = AnimateAsync(bindable);
    }

    private async Task AnimateAsync(VisualElement element)
    {
        await Task.WhenAll(
            element.FadeTo(1, Duration),
            element.ScaleTo(1, Duration, Easing.SpringOut)
        );
    }
}

/// <summary>
/// Behavior para animação de botão ao clicar
/// </summary>
public class ButtonTapAnimationBehavior : Behavior<Button>
{
    private Button? _button;

    protected override void OnAttachedTo(Button bindable)
    {
        base.OnAttachedTo(bindable);
        _button = bindable;
        _button.Pressed += OnButtonPressed;
        _button.Released += OnButtonReleased;
    }

    protected override void OnDetachingFrom(Button bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_button != null)
        {
            _button.Pressed -= OnButtonPressed;
            _button.Released -= OnButtonReleased;
        }
    }

    private async void OnButtonPressed(object? sender, EventArgs e)
    {
        if (_button != null)
        {
            await _button.ScaleTo(0.95, 50, Easing.CubicOut);
        }
    }

    private async void OnButtonReleased(object? sender, EventArgs e)
    {
        if (_button != null)
        {
            await _button.ScaleTo(1, 100, Easing.SpringOut);
        }
    }
}

/// <summary>
/// Behavior para animação de card ao clicar - Suporta Frame e Border
/// </summary>
public class CardTapAnimationBehavior : Behavior<View>
{
    private View? _view;
    private TapGestureRecognizer? _tapGesture;

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);
        _view = bindable;

        _tapGesture = new TapGestureRecognizer();
        _tapGesture.Tapped += OnViewTapped;
        _view.GestureRecognizers.Add(_tapGesture);
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);
        if (_view != null && _tapGesture != null)
        {
            _view.GestureRecognizers.Remove(_tapGesture);
            _tapGesture.Tapped -= OnViewTapped;
        }
    }

    private async void OnViewTapped(object? sender, EventArgs e)
    {
        if (_view != null)
        {
            await _view.ScaleTo(0.98, 50);
            await _view.ScaleTo(1, 100, Easing.SpringOut);
        }
    }
}

/// <summary>
/// Behavior para Skeleton Loader (animação de shimmer)
/// </summary>
public class SkeletonLoaderBehavior : Behavior<BoxView>
{
    private BoxView? _boxView;
    private bool _isAnimating;
    private CancellationTokenSource? _cancellationTokenSource;

    protected override void OnAttachedTo(BoxView bindable)
    {
        base.OnAttachedTo(bindable);
        _boxView = bindable;
        _boxView.Opacity = 0.3;

        _ = AnimateShimmerAsync();
    }

    protected override void OnDetachingFrom(BoxView bindable)
    {
        base.OnDetachingFrom(bindable);
        _isAnimating = false;

        // Cancelar animação para liberar recursos
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    private async Task AnimateShimmerAsync()
    {
        _isAnimating = true;
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            while (_isAnimating && _boxView != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                await _boxView.FadeTo(1, 800, Easing.SinInOut);

                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    break;

                await _boxView.FadeTo(0.3, 800, Easing.SinInOut);
            }
        }
        catch (OperationCanceledException)
        {
            // Animação cancelada normalmente
        }
    }
}
