using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckAndCover.Views;

public partial class Credit : Border
{
    public static readonly BindableProperty RowProperty =
        BindableProperty.Create(
            nameof(Row),
            typeof(string),
            typeof(Border),
            default(string));

    public string Row
    {
        get => (string)GetValue(RowProperty);
        set => SetValue(RowProperty, value);
    }
    
    public static readonly BindableProperty ColumnProperty =
        BindableProperty.Create(
            nameof(Column),
            typeof(string),
            typeof(Border),
            default(string));

    public string Column
    {
        get => (string)GetValue(ColumnProperty);
        set => SetValue(ColumnProperty, value);
    }
    
    public static readonly BindableProperty EmojiProperty =
        BindableProperty.Create(
            nameof(Emoji),
            typeof(string),
            typeof(Border),
            default(string));

    public string Emoji
    {
        get => (string)GetValue(EmojiProperty);
        set => SetValue(EmojiProperty, value);
    }
    
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(
            nameof(Name),
            typeof(string),
            typeof(Border),
            default(string));

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public static readonly BindableProperty RoleProperty =
        BindableProperty.Create(
            nameof(Role),
            typeof(string),
            typeof(Border),
            default(string));

    public string Role
    {
        get => (string)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    public Credit()
    {
        InitializeComponent();
        BindingContext = this;
    }
}