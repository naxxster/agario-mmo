using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; protected set; }

    enum UIType
    {
        NORMAL,
        PLAY
    };

    [SerializeField]
    public InputField NameInputField;
    [SerializeField]
    public InputField PasswordInputField;
    public Button SignInBtn;
    public Button SignUpBtn;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        SetNetworkHUD(UIType.NORMAL);
    }


    private void SetNetworkHUD(UIType uIType)
    {
        if (uIType == UIType.NORMAL)
        {
            //Init
            NameInputField.gameObject.SetActive(true);
            PasswordInputField.gameObject.SetActive(true);
            SignInBtn.gameObject.SetActive(true);
            SignUpBtn.gameObject.SetActive(true);
        }
        else if (uIType == UIType.PLAY)
        {
            //Hosting
            NameInputField.gameObject.SetActive(false);
            PasswordInputField.gameObject.SetActive(false);
            SignInBtn.gameObject.SetActive(false);
            SignUpBtn.gameObject.SetActive(false);
        }
    }

    public void OnClickSignInBtn()
    {
        string inputName = NameInputField.text;
        string inputPassword = PasswordInputField.text;

        if (ClientModule.Singleton.SignIn(inputName, inputPassword))
        {
            // Login existing server
            SetNetworkHUD(UIType.PLAY);
            ClientModule.Singleton.SignInProcess(inputName);
        }
        else
        {
            Debug.Log("Sign In Failed. inputName-" + inputName + ", inputPassword-" + inputPassword);
        }
    }

    public void OnClickSignUpBtn()
    {
        string inputName = NameInputField.text;
        string inputPassword = PasswordInputField.text;

        if (ClientModule.Singleton.SignUp(inputName, inputPassword))
        {
            Debug.Log("Sign Up Succeeded!");
        }
        else
        {
            Debug.Log("Sign Up Failed. inputName-" + inputName + ", inputPassword-" + inputPassword);
        }
    }
}
