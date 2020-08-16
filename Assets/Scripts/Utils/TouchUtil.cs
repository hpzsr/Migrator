using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchUtil : MonoBehaviour {

	static public TouchUtil s_instance = null;

	public delegate bool delegate_onTouchBegan(Vector2 pos);
	public delegate bool delegate_onTouchMoved(Vector2 pos);
	public delegate bool delegate_onTouchEnded(Vector2 pos);

	List<delegate_onTouchBegan> list_Began = new List<delegate_onTouchBegan>();
	List<delegate_onTouchMoved> list_Moved = new List<delegate_onTouchMoved>();
	List<delegate_onTouchEnded> list_Ended = new List<delegate_onTouchEnded>();

	void Start()
    {
		s_instance = this;
	}

	public void registerBegan(delegate_onTouchBegan callBack)
    {
		list_Began.Add(callBack);
	}

	public void registerMoved(delegate_onTouchMoved callBack)
	{
		list_Moved.Add(callBack);
	}

	public void registerEnded(delegate_onTouchEnded callBack)
	{
		list_Ended.Add(callBack);
	}

	public void clearAllRegister()
	{
		list_Began.Clear();
		list_Moved.Clear();
		list_Ended.Clear();
	}

	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			onTouchBegan(Input.mousePosition);
		}
		else if (Input.GetMouseButton(0))
		{
			onTouchMoved(Input.mousePosition);
		}
		else if (Input.GetMouseButtonUp(0))
		{
			onTouchEnded(Input.mousePosition);
		}
	}

	void onTouchBegan(Vector2 pos)
	{
		for(int i = 0; i < list_Began.Count; i++)
        {
			if(list_Began[i](pos))
			{
				break;
            }
        }
	}

	void onTouchMoved(Vector2 pos)
	{
		for (int i = 0; i < list_Moved.Count; i++)
		{
			if (list_Moved[i](pos))
			{
				break;
			}
		}
	}

	void onTouchEnded(Vector2 pos)
	{
		for (int i = 0; i < list_Ended.Count; i++)
		{
			if (list_Ended[i](pos))
			{
				break;
			}
		}
	}
}
