import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../api/api';

const MarketDashboard = () => {
    const navigate = useNavigate();
    const [assets, setAssets] = useState([]);
    const [selectedAssetId, setSelectedAssetId] = useState('');
    const [historyData, setHistoryData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');
    const [tradeQuantity, setTradeQuantity] = useState(1);
    const [currentPrice, setCurrentPrice] = useState(0);

    useEffect(() => {
        api.market.getAllAssets()
            .then(data => setAssets(data))
            .catch(err => console.error("Błąd assets:", err));
    }, []);

    // Obsługa zmiany selecta
    const handleAssetChange = (e) => {
        const newId = e.target.value;
        setSelectedAssetId(newId);

        if (!newId) {
            setHistoryData([]);
            setCurrentPrice(0);
        }
    };

    
    useEffect(() => {
        if (!selectedAssetId) return;

        api.market.getHistory(selectedAssetId)
            .then(data => {
                const formatted = data.map(item => ({                
                    date: item.date ,
                    price: item.price ,
                    volume: item.volume
                }));

                setHistoryData(formatted);

                if (formatted.length > 0) {
                    setCurrentPrice(formatted[0].price);
                }
            })
            .catch(err => console.error("Błąd pobierania historii:", err))
            .finally(() => setLoading(false));

    }, [selectedAssetId]);

    const handleTransaction = async (type) => {
        if (!localStorage.getItem('token')) return navigate('/login');

        setMessage(`Przetwarzanie...`);
        try {
            const result = await api.transaction.buy({
                assetId: parseInt(selectedAssetId),
                quantity: parseFloat(tradeQuantity),
                type: type
            });
            setMessage(result.message);
        } catch (err) {
            setMessage(err.message);
        }
    };

    const handleUpdatePrice = async () => {
        if (!selectedAssetId) return;
        const asset = assets.find(a => (a.id) == selectedAssetId);
        const ticker = asset?.ticker;

        setMessage(`Pobieranie danych dla ${ticker}...`);
        try {
            const res = await api.market.updatePrice(ticker);

            const oldId = selectedAssetId;
            setSelectedAssetId('');
            setTimeout(() => setSelectedAssetId(oldId), 100);
            setMessage(res.message);
        } catch (err) {
            setMessage(err.message);
        }
    };

    return (
        <div className="w-100">

            <h2 className="mb-4 fw-bold text-dark">Rynek Akcji</h2>

            <div className="row g-4">

                
                <div className="col-lg-8">

                    <div className="card shadow-sm border-0 mb-4">
                        <div className="card-body d-flex flex-column flex-md-row gap-3 align-items-center">
                            <select
                                className="form-select form-select-lg"
                                value={selectedAssetId}
                                onChange={handleAssetChange}
                            >
                                <option value="">-- Wybierz spółkę --</option>
                                {assets.map(asset => (
                                    <option key={asset.id || asset.Id} value={asset.id || asset.Id}>
                                        {asset.ticker || asset.Ticker} - {asset.fullName || asset.FullName}
                                    </option>
                                ))}
                            </select>

                            <button
                                onClick={handleUpdatePrice}
                                disabled={!selectedAssetId}
                                className="btn btn-primary btn-lg text-nowrap"
                            >
                                Aktualizuj API
                            </button>
                        </div>
                    </div>

                    <div className="card shadow-sm border-0">
                        <div className="card-header bg-white py-3">
                            <h5 className="mb-0 fw-bold text-secondary">Historia Notowań</h5>
                        </div>

                        <div className="card-body p-0">
                            {loading ? (
                                <div className="p-5 text-center text-muted">
                                    <div className="spinner-border text-primary mb-3" role="status"></div>
                                    <p>Ładowanie danych...</p>
                                </div>
                            ) : historyData.length > 0 ? (
                                <div className="table-responsive" style={{ maxHeight: '500px' }}>
                                    <table className="table table-striped table-hover mb-0 align-middle">
                                        <thead className="table-light sticky-top">
                                            <tr>
                                                <th className="py-3 ps-4">Data</th>
                                                <th className="py-3">Cena (USD)</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {historyData.map((row, idx) => (
                                                <tr key={idx}>
                                                    <td className="ps-4">{row.date}</td>
                                                    <td className="fw-bold text-success">
                                                        ${typeof row.price === 'number' ? row.price.toFixed(2) : row.price}
                                                    </td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            ) : (
                                <div className="p-5 text-center text-muted">
                                    <i className="bi bi-graph-up-arrow display-4 d-block mb-3 text-secondary opacity-25"></i>
                                    Wybierz spółkę, aby zobaczyć dane.
                                </div>
                            )}
                        </div>
                    </div>
                </div>

                <div className="col-lg-4">
                    <div className="card shadow border-0" style={{ position: 'sticky', top: '100px' }}>
                        <div className="card-header bg-white py-3 border-bottom">
                            <h5 className="mb-0 fw-bold text-dark">Panel Zlecenia</h5>
                        </div>

                        <div className="card-body p-4">
                            {!selectedAssetId ? (
                                <p className="text-muted text-center py-4">Wybierz aktywo z listy po lewej stronie, aby rozpocząć handel.</p>
                            ) : (
                                <div className="d-flex flex-column gap-3">

                                    <div>
                                        <label className="text-uppercase text-muted small fw-bold">Aktualna Cena</label>
                                        <div className="display-6 fw-bold text-primary">
                                            ${currentPrice ? currentPrice.toFixed(2) : '0.00'}
                                        </div>
                                    </div>

                                    <div>
                                        <label className="form-label fw-bold text-secondary">Ilość Akcji</label>
                                        <input
                                            type="number"
                                            min="1"
                                            value={tradeQuantity}
                                            onChange={(e) => setTradeQuantity(e.target.value)}
                                            className="form-control form-control-lg font-monospace"
                                        />
                                    </div>

                                    <div className="alert alert-secondary d-flex justify-content-between align-items-center m-0">
                                        <span className="small">Wartość:</span>
                                        <span className="fw-bold fs-5">
                                            ${(tradeQuantity * currentPrice).toFixed(2)}
                                        </span>
                                    </div>

                                    <div className="row g-2 mt-1">
                                        <div className="col-6">
                                            <button
                                                onClick={() => handleTransaction('BUY')}
                                                className="btn btn-success w-100 py-3 fw-bold shadow-sm"
                                            >
                                                KUP
                                            </button>
                                        </div>
                                        
                                    </div>

                                    {message && (
                                        <div className={`alert mt-3 text-center mb-0 ${message.includes('Błąd') ? 'alert-danger' : 'alert-success'}`}>
                                            {message}
                                        </div>
                                    )}
                                </div>
                            )}
                        </div>
                    </div>
                </div>

            </div>
        </div>
    );
};

export default MarketDashboard;