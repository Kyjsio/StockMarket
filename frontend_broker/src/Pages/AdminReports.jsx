import React, { useEffect, useState } from 'react';
import { api } from '../api/api';

const AdminReports = () => {
    const [reportData, setReportData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        loadReport();
    }, []);

    const loadReport = async () => {
        setLoading(true);
        try {
            const data = await api.admin.getUserReport();
            setReportData(data);
            setError('');
        } catch (err) {
            setError(err.message || "Błąd pobierania raportu.");
        } finally {
            setLoading(false);
        }
    };

    if (loading) return <div className="text-center mt-5"><div className="spinner-border text-primary"></div></div>;

    return (
        <div className="container mt-4 animate-fade-in ">
            <div className="d-flex justify-content-between align-items-center mt-4 mb-4">
                <div>
                    <h2 className="h3 fw-bold mb-1">Raport Finansowy Użytkowników</h2>
                </div>
               
            </div>

            {error && <div className="alert alert-danger">{error}</div>}

            <div className="card shadow-sm border-0">
                <div className="table-responsive">
                    <table className="table table-hover align-middle mb-0">
                        <thead className="table-light">
                            <tr>
                                <th className="ps-4">Użytkownik</th>
                                <th>Data Rejestracji</th>
                                <th className="text-end">Liczba Transakcji</th>
                                <th className="text-end">Saldo Gotówkowe</th>
                                <th className="text-end pe-4">Zrealizowany Zysk</th>
                            </tr>
                        </thead>
                        <tbody>
                            {reportData.map(row => (
                                <tr key={row.id}>
                                    <td className="ps-4">
                                        <div className="fw-bold text-dark">{row.email}</div>
                                        <div className="small text-muted">{row.firstName} {row.lastName}</div>
                                    </td>
                                    <td className="text-secondary small">
                                        {new Date(row.createdAt).toLocaleDateString('pl-PL')}
                                    </td>
                                    <td className="text-end">
                                        <span className="badge bg-secondary bg-opacity-10 text-secondary border">
                                            {row.transactionsCount} operacji
                                        </span>
                                    </td>
                                    <td className="text-end fw-bold text-dark">
                                        ${row.balance?.toFixed(2)}
                                    </td>
                                    <td className={`text-end pe-4 fw-bold ${row.totalRealizedProfit >= 0 ? 'text-success' : 'text-danger'}`}>
                                        {row.totalRealizedProfit > 0 ? '+' : ''}{row.totalRealizedProfit.toFixed(2)} $
                                    </td>
                                </tr>
                            ))}
                            {reportData.length === 0 && <tr><td colSpan="5" className="text-center py-5">Brak danych</td></tr>}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default AdminReports;