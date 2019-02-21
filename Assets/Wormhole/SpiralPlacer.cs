using UnityEngine;

public class SpiralPlacer : ItemGenerator {

	public TunnellItem[] itemPrefabs;

	public override void GenerateItems (Wormhole pipe)
	{
        //puts items either clockwise or counterclockwise
        float start = (Random.Range(0, pipe.pipeSegmentCount) + 0.5f);
		float direction = Random.value < 0.5f ? 1f : -1f;

		float angleStep = pipe.CurveAngle / pipe.CurveSegmentCount;


	    for (int i = 0; i < pipe.CurveSegmentCount; i++)
		{
			TunnellItem item = Instantiate<TunnellItem>(
				itemPrefabs[Random.Range(0, itemPrefabs.Length)]);
			float pipeRotation =
                //start that direcection
				(start + i * direction) * 360f / pipe.pipeSegmentCount;
			item.Position(pipe, i * angleStep, pipeRotation);
		}
	}
}