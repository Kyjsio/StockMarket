import React, { useEffect, useState } from 'react';
import { api } from '../api/api';

const AdminUsers = () => {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [, setError] = useState('');

    useEffect(() => ;
        loadUsers();
    }, []);

    const loadUsers = async () => {
        setLoading(true);
        try {
            const data = await api.admin.getUsers();
            setUsers(data);
            setError('');
        } catch (err) {
            console.error(err);
            setError('Błąd pobierania danych');
        } finally {
            setLoading(false);
        }
    }; 

    const handleDeleteUser = async (userId, userEmail) => {
        if (!window.confirm(`Czy na pewno chcesz usunąć użytkownika ${userEmail}? Ta operacja jest nieodwracalna.`)) {
            return;
        }

        try {
            const result = await api.admin.deleteUser(userId);
            alert(result.message);
            loadUsers();
        } catch (err) {
            alert(err.message || "Błąd podczas usuwania.");
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return 'Brak danych';
        return new Date(dateString).toLocaleString('pl-PL', {
            day: '2-digit', month: '2-digit', year: 'numeric',
            hour: '2-digit', minute: '2-digit'
        });
    };

    if (loading) {
        return (
            <div className="d-flex flex-column justify-content-center align-items-center flex-grow-1 mt-5 pt-5">
                <div className="spinner-border text-primary mb-3" role="status" style={{ width: '3rem', height: '3rem' }}></div>
                <h4 className="text-secondary fw-light">Pobieranie bazy użytkowników...</h4>
            </div>
        );
    }

    return (
        <div className="container mt-5 pt-4 animate-fade-in">
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h2 className="h3 text-dark fw-bold mb-1">Użytkownicy</h2>
                    <p className="text-muted mb-0">Zarządzanie kontami klientów w systemie</p>
                </div>
                
            </div>

            <div className="card shadow-sm border-0 rounded-3 overflow-hidden">
                <div className="card-header bg-white py-3 border-bottom">
                    <h5 className="mb-0 text-gray-800 fw-bold">Lista zarejestrowanych klientów</h5>
                </div>

                <div className="card-body p-0">
                    <div className="table-responsive">
                        <table className="table table-hover align-middle mb-0">
                            <thead className="bg-light text-secondary small text-uppercase">
                                <tr>
                                    <th scope="col" className="py-3 ps-4 border-0">ID</th>
                                    <th scope="col" className="py-3 border-0">Adres Email</th>
                                    <th scope="col" className="py-3 border-0">Data Rejestracji</th>
                                    <th scope="col" className="py-3 text-end border-0">Saldo</th>
                                    <th scope="col" className="py-3 pe-4 text-end border-0">Akcje</th>
                                </tr>
                            </thead>
                            <tbody>
                                {users.length > 0 ? (
                                    users.map((user) => (
                                        <tr key={user.id} style={{ cursor: 'default' }}>
                                            <td className="ps-4 fw-bold text-secondary">#{user.id}</td>
                                            <td className="fw-medium text-dark">{user.email}</td>
                                            <td className="text-muted small">{formatDate(user.createdAt)}</td>
                                            <td className="text-end">
                                                <span className={`badge rounded-pill ${user.balance >= 0 ? 'bg-success bg-opacity-10 text-success' : 'bg-danger bg-opacity-10 text-danger'} px-3 py-2 border border-${user.balance >= 0 ? 'success' : 'danger'}`}>
                                                    {user.balance.toFixed(2)} PLN
                                                </span>
                                            </td>
                                            <td className="pe-4 text-end">
                                                {user.role !== 'Admin' && (
                                                    <button
                                                        className="btn btn-outline-danger btn-sm"
                                                        onClick={() => handleDeleteUser(user.id, user.email)}
                                                        title="Usuń użytkownika"
                                                    >
                                                        <i className="bi bi-trash"></i> Usuń
                                                    </button>
                                                )}
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr><td colSpan="5" className="text-center py-5">Brak użytkowników</td></tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
                <div className="card-footer bg-white border-top-0 py-3">
                    <small className="text-muted">Wyświetlono {users.length} rekordów</small>
                </div>
            </div>
        </div>
    );
};

export default AdminUsers;