using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
    
    /// <summary>
    /// Funci�n que nos dice si el animador est� reproduciendo una animaci�n o no.
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public static bool IsPlaying(this Animator animator, int layerIndex = 0) {
        // Obtenemos el estado actual en el que se encuentra el animator
        // indicando el layer 0, que es el que se corresponde con el Base Layer.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        // Calculamos el tiempo en el que se encuentra la animaci�n multiplicando
        // la duraci�n por el momento actual normalizando los valores (entre 0 y 1).
        float currentTime = stateInfo.length * stateInfo.normalizedTime;
        // Devolvemos el resultado de comparar el tiempo actual < que el tiempo total.
        bool result = currentTime < stateInfo.length;
        return result;
    }

    /// <summary>
    /// Debe recibir un par�metro con el nombre del estado del animator, si este no coincide, devolver� false, aunque est� reproduciendo otro.
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public static bool AnimatorIsPlaying(this Animator animator, string stateName, int layerIndex = 0) {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        //bool result = stateInfo.IsName(stateName);
        // Si la animaci�n se est� reproduciendo y el estado actual se corresponde con el indicado por stateName
        // devolvemos true, si no, false.
        return animator.IsPlaying(layerIndex) && stateInfo.IsName(stateName);
    }

    /// <summary>
    /// Debe recibir el nombre de estado del animator y, si no coincide devolver� -1, de lo contrario
    /// devolver� el frame en el que se encuentra. Deber�s calcular dicho frame.
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public static int GetCurrentFrame(this Animator animator, string stateName) {
        // Obtenemos el total de frames de la animaci�n
        int animationTotalFrames = animator.GetTotalFrames(stateName);
        if (animationTotalFrames < 0) return -1;
        // Obtenemos el estado actual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //float normalizedTime = stateInfo.normalizedTime - Mathf.Floor(stateInfo.normalizedTime);
        // C�lculo de frame actual seg�n el porcentaje de animaci�n reproducido
        int currentFrame = Mathf.FloorToInt(stateInfo.normalizedTime * animationTotalFrames);
        return currentFrame;
    }

    /// <summary>
    /// Igual que el anterior, pero devolver� el total de los frames.
    /// </summary>
    /// <param name="animator"></param>
    /// <returns></returns>
    public static int GetTotalFrames(this Animator animator, string stateName) {

        // Obtenemos el estado actual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log(stateInfo.IsName(stateName));
        // Si el estado actual no es el indicado, devolvemos -1 como indicativo de error.
        if (!stateInfo.IsName(stateName)) return -1;
        // Obtenemos los clip de animaci�n del estado actual
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        // Obtenemos el frame actual a traves del c�lculo de frames totales
        // multiplicados por el tiempo normalizado del estado actual.
        // Duraci�n de la animaci�n
        float animationDuration = clipInfo[0].clip.length;
        // Frames por segundo de la animaci�n
        float animationFrameRate = clipInfo[0].clip.frameRate;
        // C�lculo de frames totales de la animaci�n
        int animationTotalFrames = Mathf.FloorToInt(animationDuration * animationFrameRate);
        return animationTotalFrames;
    }

    public static float GetAnimationLength(this Animator animator, string stateName)
    {
        // Obtenemos el estado actual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // Si el estado actual no es el indicado, devolvemos -1 como indicativo de error.
        if (!stateInfo.IsName(stateName)) return 0;
        // Obtenemos los clip de animaci�n del estado actual
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        // Duraci�n de la animaci�n
        return clipInfo[0].clip.length;
    }

    public static bool AnimationIsFinished(this Animator animator, string stateName) {
        int animationTotalFrames = animator.GetTotalFrames(stateName);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // C�lculo de frame actual seg�n el porcentaje de animaci�n reproducido
        int currentFrame = Mathf.FloorToInt(stateInfo.normalizedTime * animationTotalFrames);
        bool result = currentFrame >= animationTotalFrames;
        return result;
    }
}
