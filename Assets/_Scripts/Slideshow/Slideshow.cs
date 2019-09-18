using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    public static bool StopSlideShow { get; set; }

    [SerializeField]
    private GameObject SlideshowImages;

    [SerializeField]
    private int pauseDuration = 5;

    [SerializeField]
    private int fadeDuration = 1;

    private List<GameObject> imageList = new List<GameObject>();
    private RawImage currentImage;

    private int i = 0;

    //private List<RawImage> slideImages = new List<RawImage>();

    // Start is called before the first frame update
    void Start()
    {
        StopSlideShow = false;
        foreach(Transform child in SlideshowImages.transform)
        {
            imageList.Add(child.gameObject);
        }
        StartCoroutine(BeginSlideshowNoTransition());
        //imageList = SlideshowImages.GetComponentsInChildren<GameObject>(true).ToList();
        currentImage = imageList[0].gameObject.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator BeginSlideshow()
    {
        while (!StopSlideShow)
        {
            for (float i = fadeDuration; i >= 0; i -= 0.1f)
            {
                currentImage.color = new Color(1, 1, 1, i);
                yield return null;
            }

            yield return new WaitForSeconds(pauseDuration);

            for (float i = 0; i <= fadeDuration; i += 0.1f)
            {
                currentImage.color = new Color(1, 1, 1, i);
                yield return null;
            } 
        }
    }

    IEnumerator BeginSlideshowNoTransition()
    {
        while (!StopSlideShow)
        {
            yield return new WaitForSeconds((float)pauseDuration);
            imageList[i].SetActive(false);
            i = (i + 1) % imageList.Count;
            imageList[i].SetActive(true);
        }
    }

    //IEnumerator FadeImage(bool fadeAway)
    //{
    //    // fade from opaque to transparent
    //    if (fadeAway)
    //    {
    //        // loop over 1 second backwards
    //        for (float i = 1; i >= 0; i -= Time.deltaTime)
    //        {
    //            // set color with i as alpha
    //            currentImage.color = new Color(1, 1, 1, i);
    //            yield return null;
    //        }
    //    }
    //    // fade from transparent to opaque
    //    else
    //    {
    //        // loop over 1 second
    //        for (float i = 0; i <= 1; i += Time.deltaTime)
    //        {
    //            // set color with i as alpha
    //            currentImage.color = new Color(1, 1, 1, i);
    //            yield return null;
    //        }
    //    }
    //}
}
