using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace App.Behaviors
{
    public class ScrollViewerBehavior : Behavior<WebView>
    {
        private WebView _webView;

        public static readonly BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(double), typeof(ScrollViewerBehavior), 0.0,
                BindingMode.TwoWay, propertyChanged: OnPositionChanged);

        public double Position
        {
            get => (double)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        protected override void OnAttachedTo(WebView webView)
        {
            base.OnAttachedTo(webView);
            _webView = webView;
            webView.Navigated += OnWebViewNavigated;
        }

        protected override void OnDetachingFrom(WebView webView)
        {
            base.OnDetachingFrom(webView);
            webView.Navigated -= OnWebViewNavigated;
            _webView = null;
        }

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (_webView != null)
            {
                // Inject JavaScript to monitor scroll position
                string script = @"
                    window.addEventListener('scroll', function() {
                        invoke_cs('updateScrollPosition', window.scrollY);
                    });";
                
                _webView.Eval(script);
            }
        }

        private static void OnPositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var behavior = (ScrollViewerBehavior)bindable;
            if (behavior._webView != null)
            {
                behavior._webView.Eval($"window.scrollTo(0, {newValue});");
            }
        }
    }
}
