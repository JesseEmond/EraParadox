//
//  Drawable_ManMega_Slash.cs
//
//  Author:
//       HPSETUP3 <${AuthorEmail}>
//
//  Copyright (c) 2013 HPSETUP3
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using GREATClient.Network;
using GREATClient.BaseClass;
using GREATLib;
using Microsoft.Xna.Framework;

namespace GREATClient.GameContent.Spells
{
    public class Drawable_ManMega_Slash : DrawableSpell
    {
		private static readonly TimeSpan TIME_ALIVE = TimeSpan.FromSeconds(0.2);
		private double TimeAlive;

        public Drawable_ManMega_Slash(ClientLinearSpell spell)
			: base(spell,
			       new DrawableRectangle(
						new Rect(0,0,20,20), Color.DarkRed))
        {
			//TODO: refactor in DrawableMeleeSpell
			RemoveWhenDeleted = false;
			ApplyUpdates = false;

			TimeAlive = 0.0;
        }

		protected override void OnUpdate(GameTime dt)
		{
			base.OnUpdate(dt);

			TimeAlive += dt.ElapsedGameTime.TotalSeconds;
			if (TimeAlive > TIME_ALIVE.TotalSeconds) {
				RemoveWhenDeleted = true;
			}
		}
    }
}
