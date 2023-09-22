﻿using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Yapoml.Framework.Logging;
using Yapoml.Selenium.Components.Conditions;
using Yapoml.Selenium.Components.Conditions.Generic;
using Yapoml.Selenium.Events;
using Yapoml.Selenium.Services.Locator;

namespace Yapoml.Selenium.Components
{
    public abstract class BaseComponentConditions<TConditions> : ITextualConditions<TConditions> where TConditions : BaseComponentConditions<TConditions>
    {
        protected TConditions conditions;

        protected TimeSpan Timeout { get; }
        protected TimeSpan PollingInterval { get; }
        protected IWebDriver WebDriver { get; }
        protected IElementHandler ElementHandler { get; }
        protected IElementLocator ElementLocator { get; }
        protected IEventSource EventSource { get; }
        protected ILogger Logger { get; }

        public BaseComponentConditions(TimeSpan timeout, TimeSpan pollingInterval, IWebDriver webDriver, IElementHandler elementHandler, IElementLocator elementLocator, IEventSource eventSource, ILogger logger)
        {
            Timeout = timeout;
            PollingInterval = pollingInterval;
            WebDriver = webDriver;
            ElementHandler = elementHandler;
            ElementLocator = elementLocator;
            EventSource = eventSource;
            Logger = logger;
        }

        /// <summary>
        /// Waits until the component is displayed.
        /// </summary>
        /// <param name="timeout">How long to wait until the component is displayed.</param>
        /// <returns></returns>
        public virtual TConditions IsDisplayed(TimeSpan? timeout = null)
        {
            timeout ??= Timeout;

            Exception lastError = null;

            Dictionary<Type, uint> ignoredExceptions = new Dictionary<Type, uint> {
                { typeof(NoSuchElementException), 0 },
                { typeof(StaleElementReferenceException), 0 }
             };

            bool attempt()
            {
                try
                {
                    var element = ElementHandler.Locate();

                    if (element.Displayed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex) when (ignoredExceptions.ContainsKey(ex.GetType()))
                {
                    if (ex is StaleElementReferenceException)
                    {
                        ElementHandler.Invalidate();
                    }

                    lastError = ex;

                    ignoredExceptions[ex.GetType()]++;

                    return false;
                }
            }

            try
            {
                using (Logger.BeginLogScope($"Expect {ElementHandler.ComponentMetadata.Name} is displayed"))
                {
                    Services.Waiter.Until(attempt, timeout.Value, PollingInterval);
                }
            }
            catch (TimeoutException ex)
            {
                throw new ExpectException($"{ElementHandler.ComponentMetadata.Name} is not displayed yet '{ElementHandler.By}'.", ex);
            }

            return conditions;
        }

        /// <summary>
        /// Waits until the component is not displayed.
        /// Detached component from DOM is also considered as not displayed.
        /// </summary>
        /// <param name="timeout">How long to wait until the component is not displayed.</param>
        /// <returns></returns>
        public virtual TConditions IsNotDisplayed(TimeSpan? timeout = null)
        {
            timeout ??= Timeout;

            bool attempt()
            {
                try
                {
                    var element = ElementHandler.Locate();

                    return !element.Displayed;
                }
                catch (Exception ex) when (ex is StaleElementReferenceException || ex is NoSuchElementException)
                {
                    if (ex is StaleElementReferenceException)
                    {
                        ElementHandler.Invalidate();
                    }

                    return true;
                }
            }

            try
            {
                using (Logger.BeginLogScope($"Expect {ElementHandler.ComponentMetadata.Name} is not displayed"))
                {
                    Services.Waiter.Until(attempt, timeout.Value, PollingInterval);
                }
            }
            catch (TimeoutException ex)
            {
                throw new ExpectException($"{ElementHandler.ComponentMetadata.Name} is still displayed '{ElementHandler.By}'.", ex);
            }

            return conditions;
        }

        /// <summary>
        /// Waits until the component is appeared in the DOM.
        /// </summary>
        /// <param name="timeout">How long to wait until the component is appeared.</param>
        /// <returns></returns>
        public virtual TConditions Exists(TimeSpan? timeout = null)
        {
            timeout ??= Timeout;

            Exception lastError = null;

            Dictionary<Type, uint> ignoredExceptions = new Dictionary<Type, uint> {
                { typeof(NoSuchElementException), 0 },
                { typeof(StaleElementReferenceException), 0 }
             };

            bool attempt()
            {
                try
                {
                    var element = ElementHandler.Locate();

                    // ping element
                    var tagName = element.TagName;

                    return true;
                }
                catch (Exception ex) when (ignoredExceptions.ContainsKey(ex.GetType()))
                {
                    if (ex is StaleElementReferenceException)
                    {
                        ElementHandler.Invalidate();
                    }

                    lastError = ex;

                    ignoredExceptions[ex.GetType()]++;

                    return false;
                }
            }

            try
            {
                using (Logger.BeginLogScope($"Expect {ElementHandler.ComponentMetadata.Name} exists"))
                {
                    Services.Waiter.Until(attempt, timeout.Value, PollingInterval);
                }
            }
            catch (TimeoutException ex)
            {
                throw new ExpectException($"{ElementHandler.ComponentMetadata.Name} does not exist yet '{ElementHandler.By}'.", ex);
            }

            return conditions;
        }

        /// <summary>
        /// Waits until the component disappeared drom the DOM.
        /// </summary>
        /// <param name="timeout">How long to wait until the component disappeared.</param>
        /// <returns></returns>
        public virtual TConditions DoesNotExist(TimeSpan? timeout = null)
        {
            timeout ??= Timeout;

            bool attempt()
            {
                try
                {
                    var element = ElementHandler.Locate();

                    // ping element
                    var tagName = element.TagName;

                    return false;
                }
                catch (Exception ex) when (ex is StaleElementReferenceException || ex is NoSuchElementException)
                {
                    if (ex is StaleElementReferenceException)
                    {
                        ElementHandler.Invalidate();
                    }

                    return true;
                }
            }

            try
            {
                using (Logger.BeginLogScope($"Expect {ElementHandler.ComponentMetadata.Name} does not exist"))
                {
                    Services.Waiter.Until(attempt, timeout.Value, PollingInterval);
                }
            }
            catch (TimeoutException ex)
            {
                throw new ExpectException($"{ElementHandler.ComponentMetadata.Name} still exists '{ElementHandler.By}'.", ex);
            }

            return conditions;
        }

        /// <summary>
        ///  Waits until the component is enabled.
        /// </summary>
        /// <param name="timeout">How long to wait until the component is enabled.</param>
        /// <returns></returns>
        public virtual TConditions IsEnabled(TimeSpan? timeout = null)
        {
            timeout ??= Timeout;

            bool attempt()
            {
                return ElementHandler.Locate().Enabled;
            }

            try
            {
                using (Logger.BeginLogScope($"Expect {ElementHandler.ComponentMetadata.Name} is enabled"))
                {
                    Services.Waiter.Until(attempt, timeout.Value, PollingInterval);
                }
            }
            catch (TimeoutException ex)
            {
                throw new ExpectException($"{ElementHandler.ComponentMetadata.Name} is not enabled yet.", ex);
            }

            return conditions;
        }

        /// <summary>
        /// Various expected conditions for component's text.
        /// </summary>
        public virtual TextConditions<TConditions> Text
        {
            get
            {
                return new TextConditions<TConditions>(conditions, ElementHandler, Timeout, PollingInterval, $"text of the {ElementHandler.ComponentMetadata.Name}", Logger);
            }
        }

        /// <summary>
        /// Various expected conditions for component attributes.
        /// </summary>
        public virtual AttributesCollectionConditions<TConditions> Attributes
        {
            get
            {
                return new AttributesCollectionConditions<TConditions>(conditions, ElementHandler, Timeout, PollingInterval, Logger);
            }
        }

        /// <summary>
        /// Various expected conditions for CSS styles.
        /// </summary>
        public virtual StylesCollectionConditions<TConditions> Styles
        {
            get
            {
                return new StylesCollectionConditions<TConditions>(conditions, ElementHandler, Timeout, PollingInterval, Logger);
            }
        }

        /// <summary>
        /// Waits specified amount of time.
        /// </summary>
        /// <param name="duration">Aamount of time to wait.</param>
        /// <returns></returns>
        public virtual TConditions Elapsed(TimeSpan duration)
        {
            Thread.Sleep(duration);

            return conditions;
        }

        #region Textual Conditions

        public TConditions Is(string value, TimeSpan? timeout = default)
        {
            return Text.Is(value, timeout);
        }

        public TConditions Is(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.Is(value, comparisonType, timeout);
        }

        public TConditions IsNot(string value, TimeSpan? timeout = default)
        {
            return Text.IsNot(value, timeout);
        }

        public TConditions IsNot(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.IsNot(value, comparisonType, timeout);
        }

        public TConditions IsEmpty(TimeSpan? timeout = default)
        {
            return Text.IsEmpty(timeout);
        }

        public TConditions IsNotEmpty(TimeSpan? timeout = default)
        {
            return Text.IsNotEmpty(timeout);
        }

        public TConditions StartsWith(string value, TimeSpan? timeout = default)
        {
            return Text.StartsWith(value, timeout);
        }

        public TConditions StartsWith(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.StartsWith(value, comparisonType, timeout);
        }

        public TConditions DoesNotStartWith(string value, TimeSpan? timeout = default)
        {
            return Text.DoesNotStartWith(value, timeout);
        }

        public TConditions DoesNotStartWith(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.DoesNotStartWith(value, comparisonType, timeout);
        }

        public TConditions EndsWith(string value, TimeSpan? timeout = default)
        {
            return Text.EndsWith(value, timeout);
        }

        public TConditions EndsWith(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.EndsWith(value, comparisonType, timeout);
        }

        public TConditions DoesNotEndWith(string value, TimeSpan? timeout = default)
        {
            return Text.DoesNotEndWith(value, timeout);
        }

        public TConditions DoesNotEndWith(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.DoesNotEndWith(value, comparisonType, timeout);
        }

        public TConditions Contains(string value, TimeSpan? timeout = default)
        {
            return Text.Contains(value, timeout);
        }

        public TConditions Contains(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.Contains(value, comparisonType, timeout);
        }

        public TConditions DoesNotContain(string value, TimeSpan? timeout = default)
        {
            return Text.DoesNotContain(value, timeout);
        }

        public TConditions DoesNotContain(string value, StringComparison comparisonType, TimeSpan? timeout = default)
        {
            return Text.DoesNotContain(value, comparisonType, timeout);
        }

        public TConditions Matches(Regex regex, TimeSpan? timeout = default)
        {
            return Text.Matches(regex, timeout);
        }

        public TConditions DoesNotMatch(Regex regex, TimeSpan? timeout = default)
        {
            return Text.DoesNotMatch(regex, timeout);
        }
        #endregion
    }
}
