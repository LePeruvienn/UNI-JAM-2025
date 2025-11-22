using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RengeGames.HealthBars {

	public interface ISegmentedHealthBar {
		void SetSegmentCount(float value);
		void SetRemovedSegments(float value);
		void AddRemoveSegments(float value);
	}
}