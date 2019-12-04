using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages rating stars
///
/// Ruben Sanchez
/// </summary>
public class StarController : MonoBehaviour
{
    #region Properties

    [SerializeField] private int maxStars = 5;
    [SerializeField] private GameObject starPrefab;

    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    private List<ColorToggle> stars = new List<ColorToggle>();

    #endregion


    #region MonoBehavior Callbacks

    private void Awake()
    {
        if (layoutGroup == null)
            return;

        for (int i = 0; i < maxStars; i++)
        {
            ColorToggle color = Instantiate(starPrefab, layoutGroup.transform).GetComponent<ColorToggle>();
            color.OnToggle += SetStars;
            stars.Add(color);
        }
    }

    #endregion

    #region Public Methods

    public int GetNumberOfActiveStars()
    {
        return stars.Count(star => (!star.IsOff));
    }

    public void Reset()
    {
        foreach (var star in stars)
            star.TurnOn();
    }

    public void SetStarValue(float value)
    {
        int fullStars = (int)value;
        float partialValue = value - fullStars;
        if (fullStars > stars.Count || fullStars <= 0)
            return;

        for (int i = 0; i < stars.Count; i++)
            if (i <= fullStars) stars[i].TurnOn();
            else stars[i].TurnOff();

        if (Math.Abs(partialValue) < 0.05)
            return;

        int indexToFill = Mathf.CeilToInt(value) - 1;

        stars[indexToFill].SetFill(partialValue);
    }

    #endregion

    #region Private Methods

    private void SetStars(ColorToggle colorToggle)
    {
        int indexOfToggledStar = stars.IndexOf(colorToggle);

        foreach (var star in stars)
        {
            if (colorToggle.IsOff
                && star.transform.GetSiblingIndex() > indexOfToggledStar)
                star.TurnOff();

            else if (star.transform.GetSiblingIndex() < indexOfToggledStar)
                star.TurnOn();
        }

        colorToggle.TurnOn();
    }

    #endregion
}
