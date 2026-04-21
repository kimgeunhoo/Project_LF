using UnityEngine;

namespace BSPDungeonGenrator.Core
{
	public interface LineRenderInterface
	{
        void OnDrawLine(Vector2 from, Vector2 to);
    }

}