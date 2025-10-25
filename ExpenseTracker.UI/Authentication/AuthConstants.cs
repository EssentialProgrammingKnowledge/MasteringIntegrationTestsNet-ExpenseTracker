namespace ExpenseTracker.UI.Authentication
{
    public static class AuthConstants
    {
        // --- AUTH ----
        public const string TOKEN_STORE_NAME = "AuthToken";
        public const string AUTHENTICATION_TYPE = "Bearer";
        public const string ADMIN_ONLY_POLICY = "AdminOnlyPolicy";

        // --- CLAIMS ---
        public const string ID_CLAIM_TYPE = "sub";
        public const string NAME_CLAIM_TYPE = "name";
        public const string EMAIL_CLAIM_TYPE = "email";
        public const string FIRST_NAME_CLAIM_TYPE = "given_name";
        public const string LAST_NAME_CLAIM_TYPE = "family_name";
        public const string FULL_NAME_CLAIM_TYPE = "name";
        public const string USER_ID_CLAIM_TYPE = "user_id";
    }
}
