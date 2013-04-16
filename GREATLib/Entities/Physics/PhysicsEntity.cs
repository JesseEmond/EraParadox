//
//  PhysicsEntity.cs
//
//  Author:
//       Jesse <>
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

namespace GREATLib.Entities.Physics
{
	/// <summary>
	/// Represents an entity that is affected by physics.
	/// </summary>
    public abstract class PhysicsEntity : IEntity
    {
		private const float DEFAULT_AIR_ACCELERATION = 0.8f;
		private const float DEFAULT_HORIZONTAL_ACCELERATION = 0f;
		private const float DEFAULT_MOVE_SPEED = 250f;

		/// <summary>
		/// Gets or sets the velocity of the entity.
		/// </summary>
		/// <value>The velocity.</value>
		public Vec2 Velocity { get; set; }

		/// <summary>
		/// Gets or sets the move speed of the entity, in units per second.
		/// </summary>
		/// <value>The move speed.</value>
		public float MoveSpeed { get; set; }

		/// <summary>
		/// Gets or sets the direction in which the entity wants to move.
		/// </summary>
		/// <value>The direction.</value>
		public HorizontalDirection Direction { get; set; }

		/// <summary>
		/// Gets or sets the horizontal acceleration of the entity.
		/// The value represents the percentage of the horizontal force
		/// that stays in a frame.
		/// E.g., we keep 0.9 * the horizontal velocity every frame.
		/// </summary>
		/// <value>The horizontal acceleration.</value>
		public float HorizontalAcceleration { get; set; }

		/// <summary>
		/// Gets or sets the air acceleration.
		/// The value represents the percentage of the air control that
		/// we have every frame.
		/// E.g., we'll move at 0.8 * our movement speed every frame while in air.
		/// </summary>
		/// <value>The air acceleration.</value>
		public float AirAcceleration { get; set; }

        public PhysicsEntity()
        {
			Velocity = new Vec2();
			HorizontalAcceleration = DEFAULT_HORIZONTAL_ACCELERATION;
			MoveSpeed = DEFAULT_MOVE_SPEED;
			AirAcceleration = DEFAULT_AIR_ACCELERATION;
			Direction = HorizontalDirection.None;
        }
    }
}
