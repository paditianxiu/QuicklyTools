using System;

namespace QuicklyTools.DependencyInjection;

public sealed record ViewRegistration(Type ViewModelType, Type ViewType);
