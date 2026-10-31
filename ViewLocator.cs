using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using QuicklyTools.DependencyInjection;

namespace QuicklyTools;

/// <summary>Explicit page mappings avoid reflection and parameterless ViewModel constructors.</summary>
public sealed class ViewLocator(IServiceProvider services, IEnumerable<ViewRegistration> registrations) : IDataTemplate
{
    private readonly IReadOnlyDictionary<Type, Type> views = registrations.ToDictionary(item => item.ViewModelType, item => item.ViewType);

    public Control? Build(object? param)
    {
        if (param is null || !views.TryGetValue(param.GetType(), out var viewType))
            return null;
        var view = (Control)services.GetRequiredService(viewType);
        view.DataContext = param;
        return view;
    }

    public bool Match(object? data) => data is not null && views.ContainsKey(data.GetType());
}
