using System;

namespace Option
{
    using static Option;

    public abstract record Type();
    public record Some<T>(T Value) : Type;
    public record None() : Type;

    public static class Option
    {
        public static Option<T> None<T>() => new();
        public static Option<T> Some<T>(T value) => new(value);
    }

    /*
        Disable non exhaustive switch warnings
    */
    #pragma warning disable CS8509
    #nullable enable

    public class Option<T>
    {
        private Type _value;

        public Option(T t)
        {
            _value = new Some<T>(t);
        }

        public Option()
        {
            _value = new None();
        }

        public bool IsSome() => this._value is Some<T>;
        public bool IsNone() => this._value is None;
        public Type Case() => _value;


        public T Expect(string msg,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

        {
            if (_value is None)
            {
                Console.WriteLine($"{msg} \n Location --> File : {sourceFilePath}, " +
                        $"function: {memberName}, line: {sourceLineNumber}\n");
                throw new BadOptionAccessException();
            }
            else if (_value is Some<T>(var t))
                return t;

            throw new UnreachableException();
        }
        public T Unwrap(
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {

            if (_value is None)
            {
                Console.WriteLine($"Error unwrapping Option<{typeof(T)}> in...\n" +
                        $"Location --> File : {sourceFilePath}, function: {memberName}, " +
                        $"line: {sourceLineNumber}\n");
                throw new BadOptionAccessException();
            }
            else if (_value is Some<T>(var t))
                return t;

            throw new UnreachableException();
            
        }
        public T UnwrapOr(T default_) => _value switch
        {
            None => default_,
            Some<T>(var t) => t
        };

        public T UnwrapOrElse(Func<T> f) => _value switch
        {
            None => f(),
            Some<T>(var t) => t
        };
        public T UnwrapOrDefault() 
        {
            T? default_ = default;
            return (_value, default_) switch
            {
                (_, null) => throw new TypeNotDefautableException(),
                (None, _) => default_,
                (Some<T>(var t), _) => t,
            };
          
        }



        public Option<U> Map<U>(Func<T, U> f) => _value switch
        {
            None => None<U>(),
            Some<T>(var t) => Some(f(t))
        };
        public U MapOr<U>(Func<T, U> f, U default_) => _value switch
        {
            None => default_,
            Some<T>(var t) => f(t)
        };
        public U MapOrElse<U>(Func<T, U> f, Func<U> fallback) => _value switch
        {
            None => fallback(),
            Some<T>(var t) => f(t)
        };


        public Option<U> And<U>(Option<U> default_) => IsNone() ? None<U>() : default_; 
        public Option<U> AndThen<U>(Func<T, Option<U>> f) => _value switch
        {
            None => None<U>(),
            Some<T>(var t) => f(t)
        };


        public Option<T> Or(Option<T> u) => IsSome() ? this : u;
        public Option<T> OrElse(Func<Option<T>> f) => IsSome() ? this : f();


        public bool IsSomeAnd(Func<T, bool> f) => _value switch
        {
            None => false,
            Ok<T>(var t) => f(t)
        };
        

        public Option<T> Filter(Func<T, bool> f) => _value switch
        {
            None => None<T>(),
            Some<T>(var t) => f(t) ? this : None<T>()
        };
        public Option<(T, U)> Zip<U>(Option<U> op) => (this, op) switch
        {
            (Some<T>(var t), Some<U>(var u)) => Some((t, u)),
            _ => None<(T, U)>()
        };
        public Option<R> ZipWith<U, R>(Option<U> op, Func<T, U, R> f) => (this, op) switch
        {
            (Some<T>(var t), Some<U>(var u)) => Some(f(t, u)),
            _ => None<R>()
        };


        public override int GetHashCode() => _value switch
        {
            None => 0,
            Some<T>(var t) => t?.GetHashCode() ?? 0
        };
        public override string ToString()
        {
            if (_value is None)
            {
                return "None";
            }
            else if (_value is Some<T>(var t))
                return $"Some({t?.ToString() ?? ""})";

            throw new UnreachableException();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Option<T>)
            {
                return (this, obj as Option<T>) switch
                {
                    (Some<T>(var t), Some<T>(var d)) => t?.Equals(d) ?? false,
                    (None, None) => true,
                    _ => false
                };
            }
            return false;
        }

        public static implicit operator bool(Option<T> op) => op.IsSome();
        public static bool operator ==(Option<T> op, Option<T> op2) => op.Equals(op2);
        public static bool operator !=(Option<T> op, Option<T> op2) => !op.Equals(op2);


    }

}