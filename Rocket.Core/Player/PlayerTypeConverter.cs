﻿using System;
using System.ComponentModel;
using System.Globalization;
using Rocket.API.Player;
using Rocket.Core.DependencyInjection;

namespace Rocket.Core.Player
{
    public class PlayerTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            if (sourceType == typeof(string)) return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture, object value)
        {
            if (value is string playerId && context is UnityDescriptorContext ctx)
                return ctx.UnityContainer.Resolve<IPlayerManager>().GetPlayer(playerId);

            return base.ConvertFrom(context, culture, value);
        }
    }
}