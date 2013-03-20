//
//  StickmanChampion.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
//
//  Copyright (c) 2013 Jesse
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

namespace Champions
{
	/// <summary>
	/// The Stickman champion. Used for test purposes.
	/// </summary>
    public class StickmanChampion : Champion
    {
		public override string Name {
			get {
				return "Stickman";
			}
		}

		public override AnimationInfo StandingAnim {
			get {
				return new AnimationInfo(1, 1);
			}
		}

		public override AnimationInfo RunningAnim {
			get {
				return new AnimationInfo(3, 6);
			}
		}

		public override int CollisionHeight {
			get {
				return 40;
			}
		}

		public override int CollisionWidth {
			get {
				return 15;
			}
		}
    }
}

