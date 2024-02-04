﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Interfaces;

namespace WonderLab.Views.Controls;

public class Notification : ListBoxItem, INotification
{
    private CancellationTokenSource _cancellation = new();

    public string Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public bool CanCancelled
    {
        get => GetValue(CanCancelledProperty);
        init => SetValue(CanCancelledProperty, value);
    }

    //TODO:This Value must ms!
    public int Delay
    {
        get => GetValue(DelayProperty);
        set => SetValue(DelayProperty, value);
    }

    public static readonly StyledProperty<int> DelayProperty =
        AvaloniaProperty.Register<Notification, int>(nameof(Delay), 4000);

    public static readonly StyledProperty<string> HeaderProperty =
        AvaloniaProperty.Register<Notification, string>(nameof(Header), "Hello Header!");

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<Notification, string>(nameof(Message), "This is some Message!");

    public static readonly StyledProperty<bool> CanCancelledProperty =
        AvaloniaProperty.Register<Notification, bool>(nameof(CanCancelled), true);

    public event EventHandler Exited;

    public static Notification GetNotification(string header, string message, bool canCancelled)
    {
        return new Notification
        {
            Header = header,
            Message = message,
            CanCancelled = canCancelled
        };
    }

    public static NotificationData GetNotificationData(Notification notification)
    {
        return new NotificationData()
        {
            Header = notification.Header,
            Message = notification.Message,
        };
    }

    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        e.NameScope.Find<Button>("CloseButton")!.Click += OnClick;

        Margin = new(0, 0, 10, 15);
        await Task.Delay(Delay, _cancellation.Token)
            .ContinueWith(async x =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Margin = new(0, 0, -280, 15);
                });

                await Task.Delay(400);
                Exited?.Invoke(this, EventArgs.Empty);
            });
    }

    private async void OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using (_cancellation)
        {
            await _cancellation.CancelAsync();
        }

        _cancellation = new();
        Margin = new(0, 0, -280, 15);
        await Task.Delay(380)
            .ContinueWith(x =>
            {
                Exited?.Invoke(this, EventArgs.Empty);
            });
    }
}

public class NotificationData : INotification
{
    public string Header { get; set; }
    public string Message { get; set; }
}