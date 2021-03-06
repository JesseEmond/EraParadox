//
//  ChampionsInfo.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using GREATLib.Entities.Champions;

namespace GREATClient.GameContent
{
	/// <summary>
	/// The information of an animation of a champion.
	/// </summary>
	[Serializable()]
	public class AnimationInfo
	{
		[System.Xml.Serialization.XmlAttribute("name")]
		public ChampionAnimation Name  { get; set; }
		
		[System.Xml.Serialization.XmlAttribute("line")]
		public int Line { get; set; }

		[System.Xml.Serialization.XmlAttribute("framecount")]
		public int FrameCount { get; set; }

		[System.Xml.Serialization.XmlAttribute("framerate")]
		public int FrameRate { get; set; }

		[System.Xml.Serialization.XmlAttribute("repeat")]
		public bool Repeat { get; set; }
	}

	/// <summary>
	/// The information of an individual champion.
	/// </summary>
	[Serializable()]
	public class ChampionInfo
	{
		[System.Xml.Serialization.XmlAttribute("type")]
		public ChampionTypes Type { get; set; }

		[System.Xml.Serialization.XmlAttribute("name")]
		public string Name { get; set; }

		[System.Xml.Serialization.XmlAttribute("assetname")]
		public string AssetName { get; set; }

		[System.Xml.Serialization.XmlAttribute("framewidth")]
		public int FrameWidth { get; set; }

		[System.Xml.Serialization.XmlAttribute("frameheight")]
		public int FrameHeight { get; set; }

		[System.Xml.Serialization.XmlAttribute("portrait")]
		public string Portait { get; set; }

		[System.Xml.Serialization.XmlElement("description")]
		public string Description { get; set; }

		[System.Xml.Serialization.XmlArray("animations")]
		[System.Xml.Serialization.XmlArrayItem("animation", typeof(AnimationInfo))]
		public AnimationInfo[] Animations { get; set; }


		/// <summary>
		/// Gets the animation with the given name.
		/// </summary>
		/// <returns>The animation.</returns>
		/// <param name="name">Name.</param>
		public AnimationInfo GetAnimation(ChampionAnimation name)
		{
			foreach (AnimationInfo anim in Animations)
				if (anim.Name == name)
					return anim;

			Debug.Fail("No animation with the name " + name + " for the champion " + Name + ".");
			return null;
		}
	}

	[Serializable()]
	[System.Xml.Serialization.XmlRoot("championcollection")]
	public class ChampionInfoCollection
	{
		[System.Xml.Serialization.XmlArray("champions")]
		[System.Xml.Serialization.XmlArrayItem("champion", typeof(ChampionInfo))]
    	public ChampionInfo[] Champions { get; set; }
	}

	/// <summary>
	/// The class holding various information about all the champions
	/// in the game. Whenever a new champion is added, this list must be updated.
	/// </summary>
    public class ChampionsInfo
    {
		private Dictionary<ChampionTypes, ChampionInfo> Info { get; set; }

        public ChampionsInfo()
        {
			FillInfo();
        }

		public ChampionInfo GetInfo(ChampionTypes champion)
		{
			Debug.Assert(Info.ContainsKey(champion));
			return Info[champion];
		}

		private void FillInfo()
		{
			const string CHAMPIONS_PATH = "Content/Settings/champions.xml";
			Info = new Dictionary<ChampionTypes, ChampionInfo>();

			ChampionInfoCollection champions = null;

			XmlSerializer serializer = new XmlSerializer(typeof(ChampionInfoCollection));

			StreamReader reader = new StreamReader(CHAMPIONS_PATH);
			champions = (ChampionInfoCollection)serializer.Deserialize(reader);
			reader.Close();

			foreach (ChampionInfo info in champions.Champions)
			{
				Debug.Assert(!Info.ContainsKey(info.Type));
				Info.Add(info.Type, info);
			}
		}
    }
}
