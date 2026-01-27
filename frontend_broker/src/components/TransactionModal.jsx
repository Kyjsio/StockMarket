import React, { useState } from 'react';

const TransactionModal = ({ isOpen, onClose, type, onConfirm, loading, error }) => {
    const [amount, setAmount] = useState('');

    if (!isOpen) return null;

    const handleSubmit = (e) => {
        e.preventDefault();
        onConfirm(parseFloat(amount));
        setAmount('');
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 w-full max-w-md shadow-xl">
                <h3 className="text-xl font-bold mb-4 text-gray-800">
                    {type === 'deposit' ? 'Wpłać środki' : 'Wypłać środki'}
                </h3>

                <form onSubmit={handleSubmit}>
                    <div className="mb-4">
                        <label className="block text-sm font-medium text-gray-700 mb-1">Kwota ($)</label>
                        <input
                            type="number"
                            step="0.01"
                            min="1"
                            required
                            className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            value={amount}
                            onChange={(e) => setAmount(e.target.value)}
                            placeholder="0.00"
                        />
                    </div>

                    {error && (
                        <div className="alert alert-danger mb-3 p-2 text-sm">
                            {error}
                        </div>
                    )}

                    <div className="flex justify-end gap-3">
                        <button
                            type="button"
                            onClick={onClose}
                            className="btn btn-secondary fw-bold text-white"
                            disabled={loading}
                        >
                            Anuluj
                        </button>
                        <button
                            type="submit"
                            className="btn btn-danger fw-bold text-white"

                            >
                            {loading ? '...' : (type === 'deposit' ? 'Wpłać' : 'Wypłać')}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default TransactionModal;