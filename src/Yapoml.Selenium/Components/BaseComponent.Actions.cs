﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using Yapoml.Selenium.Options;

namespace Yapoml.Selenium.Components
{
    partial class BaseComponent<TComponent, TConditions>
    {
        /// <summary>
        /// Clears the text from a component.
        /// <para>
        /// It is useful for deleting the existing text before entering new text.
        /// For example, you can use it to erase a query in a search box, or clear a password field.
        /// </para>
        /// </summary>
        /// <returns>The same instance of the component to continue interaction with it.</returns>
        public virtual TComponent Clear()
        {
            _logger.Trace($"Clearing {Metadata.Name}");

            RelocateOnStaleReference(() => WrappedElement.Clear());

            return component;
        }

        /// <inheritdoc cref="Clear()"/>
        /// <param name="when">Condition to be satisfied before clearing a text.</param>
        public virtual TComponent Clear(Action<TConditions> when)
        {
            when(conditions);

            return Clear();
        }

        /// <summary>
        /// Sends a sequence of keystrokes to a component.
        /// <para>
        /// It is useful for entering text, selecting options, or performing keyboard shortcuts.
        /// For example, you can use it to type a query in a search box, choose a value from a dropdown menu, or press the enter key.
        /// </para>
        /// </summary>
        /// <param name="text">The text to be typed. Also supports <seealso cref="OpenQA.Selenium.Keys"/>.</param>
        /// <returns>The same instance of the component to continue interaction with it.</returns>
        public virtual TComponent Type(string text)
        {
            // todo make it event based
            if (Metadata.Name.ToLowerInvariant().Contains("password") && text != null)
            {
                _logger.Trace($"Typing '{new string('*', text.Length)}' into {Metadata.Name}");
            }
            else
            {
                _logger.Trace($"Typing '{text}' into {Metadata.Name}");
            }

            RelocateOnStaleReference(() => WrappedElement.SendKeys(text));

            return component;
        }

        /// <inheritdoc cref="Type(string)"/>
        /// <param name="when">Condition to be satisfied before simulating a mouse click.</param>
        public virtual TComponent Type(string text, Action<TConditions> when)
        {
            when(conditions);

            return Type(text);
        }

        /// <summary>
        /// Simulates a mouse click on a component. It can be used to interact with buttons, links,
        /// checkboxes, radio buttons, and other clickable components on a page.
        /// </summary>
        /// <returns>The same instance of the component to continue interaction with it.</returns>
        public virtual TComponent Click()
        {
            _logger.Trace($"Clicking on {Metadata.Name}");

            RelocateOnStaleReference(() => WrappedElement.Click());

            return component;
        }

        /// <inheritdoc cref="Click()"/>/>
        /// <param name="when">Condition to be satisfied before simulating a mouse click.</param>
        public virtual TComponent Click(Action<TConditions> when)
        {
            when(conditions);

            return Click();
        }

        /// <inheritdoc cref="Click()"/>
        /// <param name="x">Coordinates offset by X-axis.</param>
        /// <param name="y">Coordinates offset by Y-axis.</param>
        public virtual TComponent Click(int x, int y)
        {
            _logger.Trace($"Clicking on {Metadata.Name} by X: {x}, Y: {y}");

            RelocateOnStaleReference(() => new Actions(WebDriver).MoveToElement(WrappedElement, x, y).Click().Build().Perform());

            return component;
        }

        /// <inheritdoc cref="Click(int, int)"/>
        /// <inheritdoc cref="Click(Action{TConditions})"/>
        public virtual TComponent Click(int x, int y, Action<TConditions> when)
        {
            when(conditions);

            return Click(x, y);
        }

        /// <summary>
        /// Moves the cursor onto the element or one of its child elements.
        /// </summary>
        public virtual TComponent Hover()
        {
            _logger.Trace($"Hovering over {Metadata.Name}");

            RelocateOnStaleReference(() => new Actions(WebDriver).MoveToElement(WrappedElement).Build().Perform());

            return component;
        }

        /// <summary>
        /// Moves the cursor onto the element or one of its child elements.
        /// </summary>
        public virtual TComponent Hover(Action<TConditions> when)
        {
            when(conditions);

            return Hover();
        }

        /// <summary>
        /// Moves the cursor onto the element or one of its child elements.
        /// </summary>
        public virtual TComponent Hover(int x, int y)
        {
            _logger.Trace($"Hovering on {Metadata.Name} by X: {x}, Y: {y}");

            RelocateOnStaleReference(() => new Actions(WebDriver).MoveToElement(WrappedElement, x, y).Build().Perform());

            return component;
        }

        /// <summary>
        /// Moves the cursor onto the element or one of its child elements.
        /// </summary>
        public virtual TComponent Hover(int x, int y, Action<TConditions> when)
        {
            when(conditions);

            return Hover(x, y);
        }

        /// <summary>
        /// Scrolls the element's ancestor containers is visible to the user.
        /// </summary>
        public virtual TComponent ScrollIntoView()
        {
            if (SpaceOptions.Services.TryGet<ScrollIntoViewOptions>(out var options))
            {
                ScrollIntoView(options);
            }
            else
            {
                _logger.Trace($"Scrolling {Metadata.Name} into view");

                var js = "arguments[0].scrollIntoView();";

                RelocateOnStaleReference(() => (WebDriver as IJavaScriptExecutor).ExecuteScript(js, WrappedElement));
            }

            return component;
        }

        /// <summary>
        /// Scrolls the element's ancestor containers is visible to the user.
        /// </summary>
        public virtual TComponent ScrollIntoView(Action<TConditions> when)
        {
            if (SpaceOptions.Services.TryGet<ScrollIntoViewOptions>(out var options))
            {
                ScrollIntoView(options, when);
            }
            else
            {
                when(conditions);

                ScrollIntoView();
            }

            return component;
        }

        /// <summary>
        /// Scrolls the element's ancestor containers is visible to the user.
        /// </summary>
        public virtual TComponent ScrollIntoView(ScrollIntoViewOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _logger.Trace($"Scrolling {Metadata.Name} into view with options {options}");

            var js = $"arguments[0].scrollIntoView({options.ToJson()});";

            RelocateOnStaleReference(() => (WebDriver as IJavaScriptExecutor).ExecuteScript(js, WrappedElement));

            return component;
        }

        /// <summary>
        /// Scrolls the element's ancestor containers is visible to the user.
        /// </summary>
        public virtual TComponent ScrollIntoView(ScrollIntoViewOptions options, Action<TConditions> when)
        {
            when(conditions);

            return ScrollIntoView(options);
        }

        /// <summary>
        /// Sets focus on the specified element, if it can be focused.
        /// The focused element is the element that will receive keyboard and similar events by default.
        /// </summary>
        public virtual TComponent Focus()
        {
            if (SpaceOptions.Services.TryGet<FocusOptions>(out var options))
            {
                Focus(options);
            }
            else
            {
                _logger.Trace($"Focusing {Metadata.Name}");

                var js = "arguments[0].focus();";

                RelocateOnStaleReference(() => (WebDriver as IJavaScriptExecutor).ExecuteScript(js, WrappedElement));
            }

            return component;
        }

        /// <summary>
        /// Sets focus on the specified element, if it can be focused.
        /// The focused element is the element that will receive keyboard and similar events by default.
        /// </summary>
        public virtual TComponent Focus(Action<TConditions> when)
        {
            if (SpaceOptions.Services.TryGet<FocusOptions>(out var options))
            {
                return Focus(options, when);
            }
            else
            {
                when(conditions);

                return Focus();
            }
        }

        /// <summary>
        /// Sets focus on the specified element, if it can be focused.
        /// The focused element is the element that will receive keyboard and similar events by default.
        /// </summary>
        public virtual TComponent Focus(FocusOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            _logger.Trace($"Focusing {Metadata.Name} with options {options}");

            var js = $"arguments[0].focus({options.ToJson()});";

            RelocateOnStaleReference(() => (WebDriver as IJavaScriptExecutor).ExecuteScript(js, WrappedElement));

            return component;
        }

        /// <summary>
        /// Sets focus on the specified element, if it can be focused.
        /// The focused element is the element that will receive keyboard and similar events by default.
        /// </summary>
        public virtual TComponent Focus(FocusOptions options, Action<TConditions> when)
        {
            when(conditions);

            return Focus(options);
        }

        /// <summary>
        /// Removes keyboard focus from the element.
        /// </summary>
        public virtual TComponent Blur()
        {
            _logger.Trace($"Bluring {Metadata.Name}");

            var js = $"arguments[0].blur();";

            RelocateOnStaleReference(() => (WebDriver as IJavaScriptExecutor).ExecuteScript(js, WrappedElement));

            return component;
        }

        /// <summary>
        /// Removes keyboard focus from the element.
        /// </summary>
        public virtual TComponent Blur(Action<TConditions> when)
        {
            when(conditions);

            return Blur();
        }

        /// <summary>
        /// Simulates a mouse right click on a component. It can be used to interact with elements
        /// that show a context menu when right clicked, such as opening a link in a new tab, copying text.
        /// </summary>
        /// <returns>The same instance of the component to continue interaction with it.</returns>
        public virtual TComponent ContextClick()
        {
            _logger.Trace($"Context clicking on {Metadata.Name}");

            RelocateOnStaleReference(() => new Actions(WebDriver).ContextClick(WrappedElement).Build().Perform());

            return component;
        }

        /// <inheritdoc cref="ContextClick()"/>
        /// <param name="when">Condition to be satisfied before simulating a mouse right click.</param>
        public virtual TComponent ContextClick(Action<TConditions> when)
        {
            when(conditions);

            return ContextClick();
        }

        /// <summary>
        /// Simulates a mouse double click on a component. It can be used to interact with elements
        /// that require a double click to launch specific functions, such as opening a file, selecting a word of text, etc.
        /// </summary>
        /// <returns>The same instance of the component to continue interaction with it.</returns>
        public virtual TComponent DoubleClick()
        {
            _logger.Trace($"Double clicking on {Metadata.Name}");

            RelocateOnStaleReference(() => new Actions(WebDriver).DoubleClick(WrappedElement).Build().Perform());

            return component;
        }

        /// <inheritdoc cref="DoubleClick()"/>
        /// <param name="when">Condition to be satisfied before simulating a mouse double click.</param>
        public virtual TComponent DoubleClick(Action<TConditions> when)
        {
            when(conditions);

            return DoubleClick();
        }

        /// <summary>
        /// Performs a drag and drop operation to another component.
        /// </summary>
        public virtual TComponent DragAndDrop<TToComponent, TToConditions>(BaseComponent<TToComponent, TToConditions> toComponent)
            where TToComponent : BaseComponent<TToComponent, TToConditions>
            where TToConditions : BaseComponentConditions<TToConditions>
        {
            _logger.Trace($"Dragging {Metadata.Name} to {toComponent.Metadata.Name}");

            RelocateOnStaleReference(() => new Actions(WebDriver).DragAndDrop(WrappedElement, toComponent.WrappedElement).Build().Perform());

            return component;
        }

        /// <summary>
        /// Performs a drag and drop operation to another component.
        /// </summary>
        public virtual TComponent DragAndDrop<TToComponent, TToConditions>(BaseComponent<TToComponent, TToConditions> toComponent, Action<TConditions> when)
            where TToComponent : BaseComponent<TToComponent, TToConditions>
            where TToConditions : BaseComponentConditions<TToConditions>
        {
            when(conditions);

            return DragAndDrop(toComponent);
        }
    }
}
