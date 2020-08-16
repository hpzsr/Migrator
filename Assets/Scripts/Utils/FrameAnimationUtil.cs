using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void AnimationEnd();

public class FrameData
{
    public Image image;
    public string path;
    public List<Sprite> sprites = new List<Sprite>();
    public FrameAnimationUtil.FrameAnimationSpeed speed = FrameAnimationUtil.FrameAnimationSpeed.normal;
    public int curIndex = 0;
    public bool isLoop = true;
    public AnimationEnd animationEnd = null;

    public FrameData(Image _image, string _path, FrameAnimationUtil.FrameAnimationSpeed _speed, bool _isLoop = true, AnimationEnd _animationEnd = null)
    {
        image = _image;
        path = _path;
        speed = _speed;
        isLoop = _isLoop;
        animationEnd = _animationEnd;

        for (int i = 1; i < 100; i++)
        {
            Sprite sprite = Resources.Load(path + i, typeof(Sprite)) as Sprite;
            if (sprite != null)
            {
                sprites.Add(sprite);
            }
            else
            {
                break;
            }
        }

        if(sprites.Count == 0)
        {
            Debug.LogError("FrameData 动画路径不存在：" + path);
        }
        else
        {
            if(image)
            {
                image.sprite = sprites[curIndex];
            }
            else
            {
                Debug.LogError("FrameData image为空  " + path);
            }
        }
    }
}

public class FrameAnimationUtil : MonoBehaviour
{
    public enum FrameAnimationSpeed
    {
        low,
        normal,
        quick,
    }

    // 动画信息面板
    bool isShowInfo = false;
    Text infoBoard = null;

    public static FrameAnimationUtil instance = null;
    public List<FrameData> frameDataList = new List<FrameData>();

    static public FrameAnimationUtil getInstance()
    {
        if (FrameAnimationUtil.instance == null)
        {
            GameObject obj = new GameObject();
            FrameAnimationUtil.instance = obj.AddComponent<FrameAnimationUtil>();
            obj.name = "FrameAnimationUtil";
            DontDestroyOnLoad(obj);
        }

        return FrameAnimationUtil.instance;
    }

    void Start()
    {
        InvokeRepeating("Timer_low", 0.1f, 1.0f / 6.0f);
        InvokeRepeating("Timer_normal", 0.1f, 1.0f / 9.0f);
        InvokeRepeating("Timer_quick", 0.1f, 1.0f / 12.0f);

        if (isShowInfo)
        {
            GameObject pre = Resources.Load("Prefabs/UI/Text", typeof(GameObject)) as GameObject;
            infoBoard = GameObject.Instantiate(pre, GameObject.Find("Canvas_High").transform).GetComponent<Text>();

            // 打印所有动画
            {
                GameObject pre2 = Resources.Load("Prefabs/UI/Button", typeof(GameObject)) as GameObject;
                GameObject btn = GameObject.Instantiate(pre2, GameObject.Find("Canvas_High").transform);
                btn.transform.Find("Text").GetComponent<Text>().text = "打印动画";
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    for (int i = 0; i < frameDataList.Count; i++)
                    {
                        Debug.Log("动画：" + frameDataList[i].image.name + "  " + frameDataList[i].path);
                    }
                });
            }
        }
    }

    void Update()
    {
        // 清空不存在的动画
        for (int i = frameDataList.Count - 1; i >= 0; i--)
        {
            if (frameDataList[i].image == null)
            {
                frameDataList.RemoveAt(i);
            }
        }

        if (infoBoard)
        {
            int loopCount = 0;
            int notloopCount = 0;
            for (int i = 0; i < frameDataList.Count; i++)
            {
                if(frameDataList[i].isLoop)
                {
                    ++loopCount;
                }
                else
                {
                    ++notloopCount;
                }
            }
            infoBoard.text = "loop count:" + loopCount + "  notloopCount:" + notloopCount;
        }
    }

    public void startAnimation(Image _image, string path , FrameAnimationUtil.FrameAnimationSpeed _speed, bool _isLoop = true, AnimationEnd _animationEnd = null)
    {
        if(getFrameData(_image) != null)
        {
            stopAnimation(_image);
        }

        FrameData data = new FrameData(_image, path, _speed, _isLoop, _animationEnd);
        frameDataList.Add(data);
    }

    public void stopAnimation(Image _image)
    {
        FrameData data = getFrameData(_image);
        if(data != null)
        {
            data.image = null;
            //frameDataList.Remove(data);
        }
    }

    FrameData getFrameData(Image _image)
    {
        for (int i = 0; i < frameDataList.Count; i++)
        {
            if (frameDataList[i].image == _image)
            {
                return frameDataList[i];
            }
        }

        return null;
    }

    void Timer_low()
    {
        for (int i = 0; i < frameDataList.Count; i++)
        {
            if (frameDataList[i].speed == FrameAnimationUtil.FrameAnimationSpeed.low)
            {
                if (frameDataList[i].curIndex >= (frameDataList[i].sprites.Count - 1))
                {
                    if (frameDataList[i].isLoop)
                    {
                        frameDataList[i].curIndex = 0;
                    }
                    else
                    {
                        stopAnimation(frameDataList[i].image);
                        if (frameDataList[i].animationEnd != null)
                        {
                            frameDataList[i].animationEnd();
                        }
                        return;
                    }
                }
                else
                {
                    ++frameDataList[i].curIndex;
                }

                if (frameDataList[i].image)
                {
                    frameDataList[i].image.sprite = frameDataList[i].sprites[frameDataList[i].curIndex];
                }
                else
                {
                    //Debug.LogError("Timer_low image为空  " + frameDataList[i].path);
                    //stopAnimation(frameDataList[i].image);
                }
            }
        }
    }

    void Timer_normal()
    {
        for (int i = 0; i < frameDataList.Count; i++)
        {
            if (frameDataList[i].speed == FrameAnimationUtil.FrameAnimationSpeed.normal)
            {
                if (frameDataList[i].curIndex >= (frameDataList[i].sprites.Count - 1))
                {
                    if (frameDataList[i].isLoop)
                    {
                        frameDataList[i].curIndex = 0;
                    }
                    else
                    {
                        stopAnimation(frameDataList[i].image);
                        if (frameDataList[i].animationEnd != null)
                        {
                            frameDataList[i].animationEnd();
                        }
                    }
                }
                else
                {
                    ++frameDataList[i].curIndex;
                }

                if (frameDataList[i].image)
                {
                    frameDataList[i].image.sprite = frameDataList[i].sprites[frameDataList[i].curIndex];
                }
                else
                {
                    //Debug.LogError("Timer_normal image为空  " + frameDataList[i].path);
                    //stopAnimation(frameDataList[i].image);
                }
            }
        }
    }

    void Timer_quick()
    {
        for(int i = 0; i < frameDataList.Count; i++)
        {
            if(frameDataList[i].speed == FrameAnimationUtil.FrameAnimationSpeed.quick)
            {
                if(frameDataList[i].curIndex >= (frameDataList[i].sprites.Count - 1))
                {
                    if (frameDataList[i].isLoop)
                    {
                        frameDataList[i].curIndex = 0;
                    }
                    else
                    {
                        stopAnimation(frameDataList[i].image);
                        if (frameDataList[i].animationEnd != null)
                        {
                            frameDataList[i].animationEnd();
                        }
                    }
                }
                else
                {
                    ++frameDataList[i].curIndex;
                }

                if (frameDataList[i].image)
                {
                    frameDataList[i].image.sprite = frameDataList[i].sprites[frameDataList[i].curIndex];
                }
                else
                {
                    //Debug.LogError("Timer_quick image为空  " + frameDataList[i].path);
                    //stopAnimation(frameDataList[i].image);
                }
            }
        }
    }

    public void clearAll()
    {
        frameDataList.Clear();
    }
}