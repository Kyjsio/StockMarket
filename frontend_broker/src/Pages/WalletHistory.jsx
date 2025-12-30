import React, { useState, useEffect } from 'react';
import { api } from '../api/api';

const WalletHistory = () => {
    const [historyLogs, setHistoryLogs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchHistory = async () => {
            try {
                const data = await api.wallet.history();
                setHistoryLogs(data);
            } catch {
                setError('Nie udało się pobrać historii.');
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

    const getStyles = (tag) => {
        switch (tag) {
            case 'deposit':
                return {
                    badge: "bg-success bg-opacity-10 text-success border border-success",
                    text: "text-success fw-bold",
                    icon: "bi bi-arrow-down-circle"
                };
            case 'withdraw':
                return {
                    badge: "bg-danger bg-opacity-10 text-danger border border-danger",
                    text: "text-danger fw-bold",
                    icon: "bi bi-arrow-up-circle"
                };
            case 'buy':
                return {
                    badge: "bg-primary bg-opacity-10 text-primary border border-primary",
                    text: "text-danger",
                    icon: "bi bi-bag-plus"
                };
            case 'sell':
                return {
                    badge: "bg-warning bg-opacity-10 text-dark border border-warning",
                    text: "text-success",
                    icon: "bi bi-bag-dash"
                };
            default:
                return {
                    badge: "bg-secondary bg-opacity-10 text-secondary",
                    text: "text-muted",
                    icon: "bi bi-circle"
                };
        }
    };

    if (loading) {
        return (
            <div className="d-flex justify-content-center py-5">
                <div className="spinner-border text-primary" role="status"></div>
            </div>
        );
    }

    if (error) {
        return <div className="alert alert-danger text-center mt-4">{error}</div>;
    }

    return (
        <div className="w-100">
            <h2 className="text-dark fw-bold mb-4 pb-2 border-bottom">Historia Operacji</h2>

            <div className="card shadow-sm border-0 rounded-3 overflow-hidden">
                {!historyLogs || historyLogs.length === 0 ? (
                    <div className="p-5 text-center text-muted">
                        Brak historii operacji.
                    </div>
                ) : (
                    <div className="table-responsive">
                        <table className="table table-hover align-middle mb-0">
                            <thead className="table-light">
                                <tr>
                                    <th className="py-3 ps-4 text-secondary small text-uppercase">Data</th>
                                    <th className="py-3 text-center text-secondary small text-uppercase">Operacja</th>
                                    <th className="py-3 pe-4 text-end text-secondary small text-uppercase">Kwota</th>
                                </tr>
                            </thead>
                            <tbody>
                                {historyLogs.map((log) => {

                                    const style = getStyles(log.operationTag);
                                    const amountFormatted = new Intl.NumberFormat('pl-PL', {
                                        style: 'currency',
                                        currency: 'USD', 
                                        signDisplay: 'always' 
                                    }).format(log.amount); 

                                    return (
                                        <tr key={log.id}>
                                            <td className="ps-4 text-muted">
                                                {formatDate(log.date)}
                                            </td>

                                            <td className="text-center">
                                                <span className={`badge rounded-pill fw-normal px-3 py-2 d-inline-flex align-items-center gap-2 ${style.badge}`}>
                                                    <i className={style.icon}></i>
                                                    {log.title}
                                                </span>
                                            </td>

                                            <td className={`pe-4 text-end font-monospace fs-6 ${style.text}`}>
                                                {amountFormatted}
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

export default WalletHistory;