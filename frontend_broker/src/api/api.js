
const API_BASE_URL = 'http://localhost:5032/api';


const getAuthHeaders = () => {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        'Authorization': token ? `Bearer ${token}` : ''
    };
};
const handleResponse = async (response) => {
    const text = await response.text();
    let data;
    try {
        data = JSON.parse(text);
    } catch {
        data = { message: text };
    }

    if (!response.ok) {
        if (response.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login';
        }

        const errorMsg = data.message || 'Wyst¹pi³ b³¹d po³¹czenia.';
        throw new Error(errorMsg);
    }
    return data;
};

export const api = {

    auth: {
            login: async (credentials) => {
                const response = await fetch(`${API_BASE_URL}/Auth/login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(credentials)
                });

                const data = await handleResponse(response);

              
                if (data.token) {
                    localStorage.setItem('token', data.token);
                    if (data.role) {
                        localStorage.setItem('userRole', data.role);
                    }

                    if (data.email) {
                        localStorage.setItem('userEmail', data.email);
                    }
                }

                return data;
            },
        
        register: async (userData) => {
            const response = await fetch(`${API_BASE_URL}/Auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(userData)
            });
            return handleResponse(response);
        }
    },

    market: {
        getAllAssets: async () => {
            const response = await fetch(`${API_BASE_URL}/Market/assets`, {
                method: 'GET',
                headers: getAuthHeaders() 
            });
            return handleResponse(response);
        },
        getHistory: async (assetId) => {
            const response = await fetch(`${API_BASE_URL}/Market/history/${assetId}`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        },
        updatePrice: async (ticker) => {
            const response = await fetch(`${API_BASE_URL}/Market/update/${ticker}`, {
                method: 'POST',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        }
    },
    transaction: {
        buy: async (transactionData) => {
            const payload = { ...transactionData, Type: "BUY" };

            const response = await fetch(`${API_BASE_URL}/Transaction/BuyStock`, {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify(payload)
            });
            return handleResponse(response);
        },

        sell: async ({ assetId, quantity, transactionId }) => { 
            const payload = {
                AssetId: assetId,
                Quantity: quantity,
                TransactionId: transactionId 
            };

            const response = await fetch(`${API_BASE_URL}/Transaction/SellStock`, {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify(payload)
            });
            return handleResponse(response);
        },

        getClosedHistory: async () => {
            const response = await fetch(`${API_BASE_URL}/Transaction/GetClosedHistory`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        },


        getDetails: async (ticker) => {
            const response = await fetch(`${API_BASE_URL}/Transaction/details/${ticker}`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        }
    },

    portfolio: {
        get: async () => {
            const response = await fetch(`${API_BASE_URL}/Wallet`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        }
    },

    wallet: {
        deposit: async (amount) => {
            const response = await fetch(`${API_BASE_URL}/Wallet/deposit`, {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify({ amount: parseFloat(amount) })
            });
            return handleResponse(response);
        },
        withdraw: async (amount) => {
            const response = await fetch(`${API_BASE_URL}/Wallet/withdraw`, {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify({ amount: parseFloat(amount) })
            });
            return handleResponse(response);
        },
        history: async () => {
            const response = await fetch(`${API_BASE_URL}/Wallet/history`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        }
    },
    admin: {
        addAsset: async (assetData) => {

            const response = await fetch(`${API_BASE_URL}/Admin/add-asset`, {
                method: 'POST',
                headers: getAuthHeaders(),
                body: JSON.stringify(assetData)
            });
            return handleResponse(response);
        },

        getUsers: async () => {
            const response = await fetch(`${API_BASE_URL}/Admin/users`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        },
        deleteUser: async (userId) => {
            const response = await fetch(`${API_BASE_URL}/Admin/delete-user/${userId}`, {
                method: 'DELETE',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        },
        getUserReport: async () => {
            const response = await fetch(`${API_BASE_URL}/Admin/reports/users`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        },
        getSystemStats: async () => {
            const response = await fetch(`${API_BASE_URL}/Admin/reports/stats`, {
                method: 'GET',
                headers: getAuthHeaders()
            });
            return handleResponse(response);
        }
    }
};