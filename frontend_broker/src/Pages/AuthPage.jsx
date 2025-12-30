import React, { useState } from 'react';
import { api } from '../api/api';

const AuthPage = () => {
    const [isLoginView, setIsLoginView] = useState(true);
    const [formData, setFormData] = useState({
        email: '', password: '', firstName: '', lastName: ''
    });
    const [message, setMessage] = useState({ text: '', type: '' });
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage({ text: '', type: '' });
        setLoading(true);

        try {
            let data;
            if (isLoginView) {
                data = await api.auth.login({ email: formData.email, password: formData.password });

                localStorage.setItem('token', data.token);
                localStorage.setItem('userEmail', data.email);
                localStorage.setItem('userRole', data.role);

                setMessage({ text: data.message, type: 'success' });

                setTimeout(() => {
                    if (data.role === 'Admin') {
                        window.location.href = '/admin'; 
                    } else {
                        window.location.href = '/market'; 
                    }
                }, 1000);

            } else {
                // Rejestracja
                const response = await api.auth.register(formData);
                setMessage({ text: response.message, type: 'success' });

                setIsLoginView(true);
                setFormData(prev => ({ ...prev, password: '' }));
            }
        } catch (error) {
            console.error(error);
            setMessage({ text: error.message || 'Wystąpił błąd', type: 'error' });
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center w-100" style={{ minHeight: '80vh' }}>
            <div className="card shadow p-4 border-0" style={{ maxWidth: '400px', width: '100%' }}>
                <h2 className="text-center mb-4 fw-bold text-secondary">
                    {isLoginView ? 'Zaloguj się' : 'Utwórz konto'}
                </h2>

                {message.text && (
                    <div className={`alert ${message.type === 'error' ? 'alert-danger' : 'alert-success'} text-center`} role="alert">
                        {message.text}
                    </div>
                )}

                <form onSubmit={handleSubmit}>
                    {!isLoginView && (
                        <>
                            <div className="mb-3">
                                <input type="text" name="firstName" placeholder="Imię" onChange={handleChange} className="form-control py-2" />
                            </div>
                            <div className="mb-3">
                                <input type="text" name="lastName" placeholder="Nazwisko" onChange={handleChange} className="form-control py-2" />
                            </div>
                        </>
                    )}
                    <div className="mb-3">
                        <input type="email" name="email" placeholder="Adres Email" required onChange={handleChange} className="form-control py-2" />
                    </div>
                    <div className="mb-4">
                        <input type="password" name="password" placeholder="Hasło" required minLength={6} onChange={handleChange} className="form-control py-2" />
                    </div>

                    <button type="submit" disabled={loading} className="btn btn-primary w-100 py-2 fw-bold">
                        {loading ? (
                            <>
                                <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                Przetwarzanie...
                            </>
                        ) : (
                            isLoginView ? 'Zaloguj się' : 'Zarejestruj się'
                        )}
                    </button>
                </form>

                <div className="mt-4 text-center text-muted small">
                    <p className="mb-0">
                        {isLoginView ? 'Nie masz konta?' : 'Masz już konto?'}
                        <button
                            onClick={() => setIsLoginView(!isLoginView)}
                            className="btn btn-link text-decoration-none fw-bold p-0 ms-1 align-baseline"
                        >
                            {isLoginView ? 'Zarejestruj się' : 'Zaloguj się'}
                        </button>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;