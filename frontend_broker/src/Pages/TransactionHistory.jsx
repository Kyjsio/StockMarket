import React, { useState, useEffect } from 'react';
import { api } from '../api/api';
import { useNavigate } from 'react-router-dom';

const TransactionHistory = () => {
    const [transactions, setTransactions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const data = await api.transaction.getClosedHistory();
                setTransactions(data);
            } catch (err) {
                setError('Nie udało się pobrać historii transakcji.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchHistory();
    }, []);

    const formatDate = (dateString) => {
        if (!dateString) return "-";
        return new Date(dateString).toLocaleString('pl-PL', {
            day: '2-digit', month: '2-digit', year: 'numeric',
            hour: '2-digit', minute: '2-digit'
        });
    };

    if (loading) {
        return (
            <div className="d-flex justify-content-center align-items-center w-100 py-5">
                <div className="spinner-border text-primary me-2" role="status"></div>
                <span className="text-secondary">Ładowanie historii...</span>
            </div>
        );
    }
    if (error) {
        return (
            <div className="container mt-5">
                <div className="alert alert-danger text-center">{error}</div>
            </div>
        );
    }

    return (
        <div className="container py-4 m-4">

            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h2 className="fw-bold text-dark mb-0">Historia Sprzedaży</h2>
                    <p className="text-muted mb-0">Lista zamkniętych pozycji i zrealizowanych zysków</p>
                </div>
                <button onClick={() => navigate('/')} className="btn btn-outline-secondary btn-sm">
                    Wróć do pulpitu
                </button>
            </div>

            <div className="card shadow-sm border-0 rounded-3 overflow-hidden">
                {!transactions || transactions.length === 0 ? (
                    <div className="p-5 text-center text-muted">
                        <i className="bi bi-inbox display-4 d-block mb-3 opacity-25"></i>
                        Brak zamkniętych pozycji w historii.
                    </div>
                ) : (
                    <div className="table-responsive">
                        <table className="table table-hover align-middle mb-0">
                            <thead className="table-light">
                                <tr>
                                    <th className="py-3 ps-4 text-secondary small text-uppercase">Data Sprzedaży</th>
                                    <th className="py-3 text-secondary small text-uppercase">Symbol</th>
                                    <th className="py-3 text-secondary small text-uppercase">Spółka</th>
                                    <th className="py-3 text-secondary small text-uppercase">Ilość</th>
                                    <th className="py-3 text-secondary small text-uppercase">Cena Sprzedaży</th>
                                    <th className="py-3 text-secondary small text-uppercase">Wartość</th>
                                    <th className="py-3 pe-4 text-end text-secondary small text-uppercase">Zysk / Strata</th>
                                </tr>
                            </thead>
                            <tbody>
                                {transactions.map((tx) => {
                                    const profitVal = tx.profit;
                                    const dateVal =  tx.date;
                                    const tickerVal = tx.ticker;
                                    const nameVal =  tx.companyName;
                                    const qtyVal =  tx.quantity;
                                    const priceVal =  tx.price;
                                    const totalVal = tx.totalAmount;
                                    const txId =  tx.id ;

                                    const profitClass = profitVal > 0
                                        ? "text-success fw-bold"
                                        : (profitVal < 0 ? "text-danger fw-bold" : "text-muted");

                                    const profitSign = profitVal > 0 ? "+" : "";

                                    return (
                                        <tr key={txId}>
                                            <td className="ps-4 text-muted">
                                                {formatDate(dateVal)}
                                            </td>
                                            <td className="fw-bold text-dark">
                                                {tickerVal}
                                            </td>
                                            <td className="text-muted small">
                                                {nameVal}
                                            </td>
                                            <td>
                                                {qtyVal} szt.
                                            </td>
                                            <td className="text-muted">
                                                ${priceVal?.toFixed(2)}
                                            </td>
                                            <td className="fw-bold">
                                                ${totalVal?.toFixed(2)}
                                            </td>

                                            <td className={`pe-4 text-end ${profitClass}`}>
                                                {profitSign}{profitVal?.toFixed(2)} $
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </div>
    );
};

export default TransactionHistory;