import React, { useState, useEffect, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../api/api';

const PositionDetailsPage = () => {
    const { ticker } = useParams();

    const [positionData, setPositionData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [actionLoading, setActionLoading] = useState(null);

    const fetchData = useCallback(async () => {
        try {
            setLoading(true);
            setError('');
            const data = await api.transaction.getDetails(ticker);
            console.log("Dane z API (Gotowe obliczenia):", data);
            setPositionData(data);
        } catch (err) {
            console.error(err);
            setError('Nie udało się pobrać danych pozycji.');
        } finally {
            setLoading(false);
        }
    }, [ticker]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const handleSellTransaction = async (tx) => {
        if (!window.confirm(`Czy na pewno chcesz sprzedać ${tx.quantity} szt. pozycji?`)) return;
        try {
            setActionLoading(tx.id);
            await api.transaction.sell({
                transactionId: tx.id,
                assetId: positionData.assetId,
                quantity: tx.quantity
            });
            await fetchData();
        } catch (err) {
            alert("Błąd: " + (err.message || "Wystąpił problem podczas sprzedaży."));
        } finally {
            setActionLoading(null);
        }
    };

    const formatDate = (dateString) => {
        if (!dateString) return "-";
        return new Date(dateString).toLocaleString('pl-PL', {
            year: 'numeric', month: '2-digit', day: '2-digit',
            hour: '2-digit', minute: '2-digit'
        });
    };

    const safeNumber = (value) => {
        const num = Number(value);
        return isNaN(num) ? 0 : num;
    };

    if (loading && !positionData) return <div className="text-center mt-5"><div className="spinner-border text-primary"></div></div>;
    if (!positionData) return <div className="text-center mt-5 p-5">Brak danych dla tego waloru.</div>;

    const totalProfit = safeNumber(positionData.profit);


    return (
        <div className="container py-4 mt-3 animate-fade-in">
            <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3 mt-4">

                <div>
                    <h2 className="fw-bold mb-0">
                        {positionData.ticker} <span className="text-muted fw-light">| Szczegóły</span>
                    </h2>
                    <span className="text-muted small">{positionData.companyName}</span>
                </div>

                <div className="bg-light p-3 rounded border shadow-sm" style={{ minWidth: '250px' }}>
                    <small className="text-muted d-block text-uppercase fw-bold mb-2 border-bottom pb-1">
                        Podsumowanie Pozycji
                    </small>

                    <div className="d-flex justify-content-between mb-1">
                        <span className="text-muted">Ilość:</span>
                        <span className="fw-bold text-dark">{positionData.currentQuantity} szt.</span>
                    </div>

                    <div className="d-flex justify-content-between mb-1">
                        <span className="text-muted">Śr. cena zakupu:</span>
                        <span className="fw-bold text-dark">${safeNumber(positionData.averageCost).toFixed(2)}</span>
                    </div>

                    <div className="d-flex justify-content-between mb-2">
                        <span className="text-muted">Wartość rynkowa:</span>
                        <span className="fw-bold text-dark">${safeNumber(positionData.currentValue).toFixed(2)}</span>
                    </div>

                    <div className="pt-2 border-top text-end">
                        <span className="text-muted small d-block">Niezrealizowany Zysk/Strata</span>

                        <div className={`fs-4 fw-bold lh-1 ${totalProfit >= 0 ? 'text-success' : 'text-danger'}`}>
                            {totalProfit > 0 ? "+" : ""}
                            ${totalProfit.toFixed(2)}
                        </div>

                       
                    </div>
                </div>
            </div>

            {error && <div className="alert alert-danger shadow-sm">{error}</div>}

            <div className="card shadow-sm border-0 overflow-hidden">
                <div className="card-header bg-white py-3">
                    <h5 className="mb-0 fw-bold text-secondary">Historia Zakupów</h5>
                </div>
                <div className="table-responsive">
                    <table className="table table-hover align-middle mb-0">
                        <thead className="table-light text-secondary small text-uppercase">
                            <tr>
                                <th className="ps-4 py-3">Data</th>
                                <th className="py-3">Typ</th>
                                <th className="py-3">Ilość</th>
                                <th className="py-3">Cena Kupna</th>
                                <th className="py-3">Koszt </th>
                                <th className="py-3">Zyski/Straty</th>
                                <th className="text-end pe-4 py-3">Akcja</th>
                            </tr>
                        </thead>
                        <tbody>
                            {positionData.transactions && positionData.transactions.map((tx) => {
                                const isBuy = tx.type === 'BUY';


                                const txProfit = safeNumber(tx.profit);
                                const txProfitPct = safeNumber(tx.profitPercentage);

                                return (
                                    <tr key={tx.id}>
                                        <td className="ps-4 text-muted">{formatDate(tx.date)}</td>

                                        <td>
                                            {isBuy ?
                                                <span className="badge bg-success bg-opacity-10 text-success border border-success px-2">KUPNO</span>
                                                : <span className="badge bg-secondary">INNE</span>
                                            }
                                        </td>

                                        <td className="fw-bold">{tx.quantity} szt.</td>
                                        <td>${safeNumber(tx.price).toFixed(2)}</td>
                                        <td className="text-muted">${safeNumber(tx.totalAmount).toFixed(2)}</td>

                                        <td>
                                            <div className={`fw-bold ${txProfit >= 0 ? "text-success" : "text-danger"}`}>
                                                {txProfit > 0 ? "+" : ""}{txProfit.toFixed(2)} $
                                            </div>
                                            <div className={`small ${txProfitPct >= 0 ? "text-success" : "text-danger"}`} style={{ fontSize: '0.85em' }}>
                                                ({txProfitPct > 0 ? "+" : ""}{txProfitPct.toFixed(2)}%)
                                            </div>
                                        </td>

                                        <td className="text-end pe-4">
                                            {tx.canSell ? (
                                                <button
                                                    onClick={() => handleSellTransaction(tx)}
                                                    disabled={actionLoading === tx.id}
                                                    className="btn btn-outline-danger btn-sm text-nowrap shadow-sm"
                                                    title="Sprzedaj tylko tę ilość akcji"
                                                >
                                                    {actionLoading === tx.id ?
                                                        <span className="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
                                                        : "Sprzedaj"
                                                    }
                                                </button>
                                            ) : (
                                                <span className="text-muted small fst-italic">Zrealizowano</span>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}

                            {(!positionData.transactions || positionData.transactions.length === 0) && (
                                <tr><td colSpan="7" className="text-center py-5 text-muted">Brak aktywnych aktywów</td></tr>
                            )}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default PositionDetailsPage;