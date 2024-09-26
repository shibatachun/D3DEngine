﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace D3DengineEditor.Utilities.Controls
{
    [TemplatePart(Name ="PART_textBlock", Type = typeof(TextBox))]
    [TemplatePart(Name ="PART_textBox", Type = typeof(TextBox))]
  
    class NumberBox : Control
    {
        private double _originalValue;

        private double _mouseStart;

        private double _multiplier;

        private bool _captrued = false;


        private bool _valueChanged = false;
        public double Multiplier
        {
            get => (double)GetValue(MultiplierProperty);
            set => SetValue(MultiplierProperty, value);
        }
        public static readonly DependencyProperty MultiplierProperty =
            DependencyProperty.Register(nameof(Multiplier), typeof(double), typeof(NumberBox),
                 new PropertyMetadata(1.0));

        //一个值看是否value有没有改变
     
        public string Value { 
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(NumberBox),
                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if(GetTemplateChild("PART_textBlock") is TextBlock textBlock)
            {
                textBlock.MouseLeftButtonDown += OnTextBlock_Mouse_LBD;
                textBlock.MouseLeftButtonUp += OnTextBlock_Mouse_LBU;
                textBlock.MouseMove += OnTextBlock_Mouse_Move;
            }
        }

        private void OnTextBlock_Mouse_Move(object sender, MouseEventArgs e)
        {
            if(_captrued)
            {
                var mouseX = e.GetPosition(this).X;
                var d = mouseX - _mouseStart;
                if (Math.Abs(d)> SystemParameters.MinimumHorizontalDragDistance)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _multiplier = 0.001;
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _multiplier = 0.1;
                    else _multiplier = 0.1;
                    var newValue = _originalValue + (d * _multiplier * Multiplier);
                    
                    Value = newValue.ToString("0.#####");
                    _valueChanged = true;
                }
            }
        }

        private void OnTextBlock_Mouse_LBU(object sender, MouseButtonEventArgs e)
        {
            if (_captrued)
            {
                Mouse.Capture(null);
                _captrued = false;
                e.Handled = true;
                if(!_valueChanged && GetTemplateChild("PART_textBox") is TextBox textBox)
                {
                    textBox.Visibility = Visibility.Visible;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void OnTextBlock_Mouse_LBD(object sender, MouseButtonEventArgs e)
        {
            double.TryParse(Value, out _originalValue);
            Mouse.Capture(sender as UIElement);
            _captrued = true;   
            _valueChanged = false;
            e.Handled = true;

            _multiplier = 0.01;
            _mouseStart = e.GetPosition(this).X;

            
        }

        static NumberBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumberBox),
                new FrameworkPropertyMetadata(typeof(NumberBox)));
        }


    }
}
