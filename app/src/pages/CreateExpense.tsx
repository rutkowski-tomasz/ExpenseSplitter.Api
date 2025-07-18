import { useState } from 'react';
import { ArrowLeft, Plus, Minus, Calculator } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Checkbox } from '@/components/ui/checkbox';
import { useToast } from '@/hooks/use-toast';

// Mock participants for development
const mockParticipants = [
  { id: '1', name: 'You' },
  { id: '2', name: 'Alice' },
  { id: '3', name: 'Bob' },
  { id: '4', name: 'Carol' },
];

interface CreateExpenseProps {
  settlementId: string;
  onNavigate: (page: string, data?: any) => void;
}

export function CreateExpense({ settlementId, onNavigate }: CreateExpenseProps) {
  const [expenseName, setExpenseName] = useState('');
  const [amount, setAmount] = useState('');
  const [paidBy, setPaidBy] = useState('1');
  const [splitMethod, setSplitMethod] = useState<'equal' | 'custom'>('equal');
  const [selectedParticipants, setSelectedParticipants] = useState<string[]>(['1', '2', '3', '4']);
  const [customAmounts, setCustomAmounts] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(false);
  const { toast } = useToast();

  const totalAmount = parseFloat(amount) || 0;
  const customTotal = Object.values(customAmounts).reduce((sum, val) => sum + (parseFloat(val) || 0), 0);
  const equalShare = selectedParticipants.length > 0 ? totalAmount / selectedParticipants.length : 0;

  const handleParticipantToggle = (participantId: string) => {
    setSelectedParticipants(prev => 
      prev.includes(participantId) 
        ? prev.filter(id => id !== participantId)
        : [...prev, participantId]
    );
  };

  const handleCustomAmountChange = (participantId: string, value: string) => {
    setCustomAmounts(prev => ({ ...prev, [participantId]: value }));
  };

  const handleSubmit = async () => {
    if (!expenseName.trim() || !amount || selectedParticipants.length === 0) {
      toast({
        title: "Missing information",
        description: "Please fill in all required fields",
        variant: "destructive",
      });
      return;
    }

    if (splitMethod === 'custom' && Math.abs(customTotal - totalAmount) > 0.01) {
      toast({
        title: "Amount mismatch",
        description: "Custom amounts don't add up to the total",
        variant: "destructive",
      });
      return;
    }

    setLoading(true);
    
    try {
      // Mock API call - replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      toast({
        title: "Expense added!",
        description: `${expenseName} has been added to the settlement`,
      });
      
      onNavigate('settlement', { id: settlementId });
    } catch (error) {
      toast({
        title: "Error",
        description: "Failed to add expense. Please try again.",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center gap-3">
          <Button 
            variant="ghost" 
            size="icon"
            onClick={() => onNavigate('settlement', { id: settlementId })}
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          
          <div>
            <h1 className="text-xl font-bold text-foreground">Add Expense</h1>
            <p className="text-sm text-muted-foreground">Split a new expense</p>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        {/* Basic Info */}
        <Card className="shadow-card border-0">
          <CardHeader>
            <CardTitle>Expense Details</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <label className="text-sm font-medium text-foreground block mb-2">
                Description
              </label>
              <Input
                type="text"
                placeholder="What was this expense for?"
                value={expenseName}
                onChange={(e) => setExpenseName(e.target.value)}
                className="h-12 rounded-xl"
              />
            </div>
            
            <div>
              <label className="text-sm font-medium text-foreground block mb-2">
                Amount
              </label>
              <div className="relative">
                <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground">$</span>
                <Input
                  type="number"
                  step="0.01"
                  placeholder="0.00"
                  value={amount}
                  onChange={(e) => setAmount(e.target.value)}
                  className="h-12 rounded-xl pl-8"
                />
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Who Paid */}
        <Card className="shadow-card border-0">
          <CardHeader>
            <CardTitle>Who paid?</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {mockParticipants.map(participant => (
                <div 
                  key={participant.id}
                  className={`flex items-center gap-3 p-3 rounded-xl cursor-pointer transition-all ${
                    paidBy === participant.id ? 'bg-primary-light border-2 border-primary' : 'bg-muted hover:bg-primary-light'
                  }`}
                  onClick={() => setPaidBy(participant.id)}
                >
                  <Avatar className="w-10 h-10">
                    <AvatarFallback className="bg-primary-light text-primary">
                      {participant.name.charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  <span className="font-medium">{participant.name}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Split Method */}
        <Card className="shadow-card border-0">
          <CardHeader>
            <CardTitle>How to split?</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="flex gap-3">
              <Button
                variant={splitMethod === 'equal' ? 'default' : 'secondary'}
                onClick={() => setSplitMethod('equal')}
                className="flex-1"
              >
                Split Equally
              </Button>
              <Button
                variant={splitMethod === 'custom' ? 'default' : 'secondary'}
                onClick={() => setSplitMethod('custom')}
                className="flex-1"
              >
                <Calculator className="mr-2 w-4 h-4" />
                Custom
              </Button>
            </div>

            <div className="space-y-3">
              {mockParticipants.map(participant => (
                <div key={participant.id} className="flex items-center gap-3 p-3 bg-muted rounded-xl">
                  <Checkbox
                    checked={selectedParticipants.includes(participant.id)}
                    onCheckedChange={() => handleParticipantToggle(participant.id)}
                  />
                  
                  <Avatar className="w-8 h-8">
                    <AvatarFallback className="text-xs bg-primary-light text-primary">
                      {participant.name.charAt(0).toUpperCase()}
                    </AvatarFallback>
                  </Avatar>
                  
                  <span className="flex-1 font-medium">{participant.name}</span>
                  
                  <div className="text-right">
                    {splitMethod === 'equal' ? (
                      <div className="text-sm font-medium">
                        {selectedParticipants.includes(participant.id) ? `$${equalShare.toFixed(2)}` : '$0.00'}
                      </div>
                    ) : (
                      <div className="relative w-20">
                        <span className="absolute left-2 top-1/2 transform -translate-y-1/2 text-xs text-muted-foreground">$</span>
                        <Input
                          type="number"
                          step="0.01"
                          placeholder="0.00"
                          value={customAmounts[participant.id] || ''}
                          onChange={(e) => handleCustomAmountChange(participant.id, e.target.value)}
                          className="h-8 text-sm pl-6 pr-2"
                          disabled={!selectedParticipants.includes(participant.id)}
                        />
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>

            {splitMethod === 'custom' && (
              <div className="p-3 bg-primary-light rounded-xl">
                <div className="flex justify-between text-sm">
                  <span>Total allocated:</span>
                  <span className={customTotal === totalAmount ? 'text-green-600' : 'text-red-600'}>
                    ${customTotal.toFixed(2)} / ${totalAmount.toFixed(2)}
                  </span>
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Submit Button */}
        <Button
          onClick={handleSubmit}
          disabled={loading || !expenseName.trim() || !amount || selectedParticipants.length === 0}
          className="w-full h-14"
        >
          {loading ? (
            <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
          ) : (
            <>
              <Plus className="mr-2 w-5 h-5" />
              Add Expense
            </>
          )}
        </Button>
      </div>
    </div>
  );
}